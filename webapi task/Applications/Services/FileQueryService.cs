using webapi_task.Applications.Interfaces;
using webapi_task.Infrastructure;
using webapi_task.Infrastructure.Interfaces;

namespace webapi_task.Applications.Services;
public class FileQueryService : IFileQueryService
{
    private readonly IUnitOfWork _unitOfWork;

    public FileQueryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Result>> GetFilteredResultsAsync(
        string? fileName = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null,
        double? averageValueFrom = null,
        double? averageValueTo = null,
        double? averageExecutionTimeFrom = null,
        double? averageExecutionTimeTo = null)
    {
        return await _unitOfWork.Results.GetFilteredAsync(
            fileName,
            startDateFrom,
            startDateTo,
            averageValueFrom,
            averageValueTo,
            averageExecutionTimeFrom,
            averageExecutionTimeTo);
    }

    public async Task<IEnumerable<Value>> GetLastValuesAsync(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name is required", nameof(fileName));
        }

        // Приводим к нижнему регистру и добавляем .csv если нужно
        fileName = fileName.ToLower();

        if (!fileName.EndsWith(".csv"))
        {
            fileName = fileName + ".csv";
        }

        return await _unitOfWork.Values.GetLastValuesByFileNameAsync(fileName, 10);
    }
}
