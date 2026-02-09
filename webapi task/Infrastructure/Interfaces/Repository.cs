using webapi_task.Infrastructure;

namespace webapi_task.Infrastructure.Interfaces
{
    public interface IValueRepository
    {
        Task AddAsync(Value value);
        Task AddRangeAsync(IEnumerable<Value> values);
        Task<IEnumerable<Value>> GetByFileNameAsync(string fileName);
        Task<int> GetCountByFileNameAsync(string fileName);
        Task DeleteByFileNameAsync(string fileName);
        Task<bool> ExistsByFileNameAsync(string fileName);
        Task<IEnumerable<Value>> GetLastValuesByFileNameAsync(string fileName, int count);
    }

    public interface IResultRepository
    {
        Task AddAsync(Result result);
        Task<Result?> GetByFileNameAsync(string fileName);
        Task DeleteByFileNameAsync(string fileName);
        Task<bool> ExistsByFileNameAsync(string fileName);

        Task<IEnumerable<Result>> GetFilteredAsync(
        string? fileName = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null,
        double? averageValueFrom = null,
        double? averageValueTo = null,
        double? averageExecutionTimeFrom = null,
        double? averageExecutionTimeTo = null);
    }

    public interface IUnitOfWork : IDisposable
    {
        IValueRepository Values { get; }
        IResultRepository Results { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
