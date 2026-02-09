using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using webapi_task.Infrastructure;
using webapi_task.Infrastructure.Interfaces;

namespace webapi_task.Infrastructure.Implementations
{
    public class ValueRepository : IValueRepository
    {
        private readonly ApplicationDbContext _context;

        public ValueRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Value value)
        {
            await _context.Values.AddAsync(value);
        }

        public async Task AddRangeAsync(IEnumerable<Value> values)
        {
            await _context.Values.AddRangeAsync(values);
        }

        public async Task<IEnumerable<Value>> GetByFileNameAsync(string fileName)
        {
            return await _context.Values
                .Where(v => v.FileName == fileName)
                .OrderBy(v => v.Date)
                .ToListAsync();
        }

        public async Task<int> GetCountByFileNameAsync(string fileName)
        {
            return await _context.Values
                .CountAsync(v => v.FileName == fileName);
        }

        public async Task DeleteByFileNameAsync(string fileName)
        {
            var values = await _context.Values
                .Where(v => v.FileName == fileName)
                .ToListAsync();

            _context.Values.RemoveRange(values);
        }

        public async Task<bool> ExistsByFileNameAsync(string fileName)
        {
            return await _context.Values
                .AnyAsync(v => v.FileName == fileName);
        }

        public async Task<IEnumerable<Value>> GetLastValuesByFileNameAsync(string fileName, int count)
        {
            // Оптимизированный запрос - одна сортировка в БД
            var lastValues = await _context.Values
                .Where(v => v.FileName == fileName)
                .OrderByDescending(v => v.Date)
                .Take(count)
                .ToListAsync();

            // Сортируем в памяти по возрастанию даты (10 записей - это мало)
            return lastValues.OrderBy(v => v.Date).ToList();
        }
    }


    public class ResultRepository : IResultRepository
    {
        private readonly ApplicationDbContext _context;

        public ResultRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Result result)
        {
            await _context.Results.AddAsync(result);
        }

        public async Task<Result?> GetByFileNameAsync(string fileName)
        {
            return await _context.Results
                .FirstOrDefaultAsync(r => r.FileName == fileName);
        }

        public async Task DeleteByFileNameAsync(string fileName)
        {
            var result = await GetByFileNameAsync(fileName);
            if (result != null)
            {
                _context.Results.Remove(result);
            }
        }

        public async Task<bool> ExistsByFileNameAsync(string fileName)
        {
            return await _context.Results
                .AnyAsync(r => r.FileName == fileName);
        }

        public async Task<IEnumerable<Result>> GetFilteredAsync(
        string? fileName = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null,
        double? averageValueFrom = null,
        double? averageValueTo = null,
        double? averageExecutionTimeFrom = null,
        double? averageExecutionTimeTo = null)
        {
            var query = _context.Results.AsQueryable();

            // Фильтр по имени файла
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                query = query.Where(r => r.FileName == fileName);
            }

            // Фильтр по времени запуска первой операции (диапазон)
            if (startDateFrom.HasValue)
            {
                query = query.Where(r => r.FirstOperationStart >= startDateFrom.Value);
            }

            if (startDateTo.HasValue)
            {
                query = query.Where(r => r.FirstOperationStart <= startDateTo.Value);
            }

            // Фильтр по среднему показателю (диапазон)
            if (averageValueFrom.HasValue)
            {
                query = query.Where(r => r.AverageValue >= averageValueFrom.Value);
            }

            if (averageValueTo.HasValue)
            {
                query = query.Where(r => r.AverageValue <= averageValueTo.Value);
            }

            // Фильтр по среднему времени выполнения (диапазон)
            if (averageExecutionTimeFrom.HasValue)
            {
                query = query.Where(r => r.AverageExecutionTime >= averageExecutionTimeFrom.Value);
            }

            if (averageExecutionTimeTo.HasValue)
            {
                query = query.Where(r => r.AverageExecutionTime <= averageExecutionTimeTo.Value);
            }

            return await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        private IValueRepository? _valueRepository;
        private IResultRepository? _resultRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IValueRepository Values =>
            _valueRepository ??= new ValueRepository(_context);

        public IResultRepository Results =>
            _resultRepository ??= new ResultRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
