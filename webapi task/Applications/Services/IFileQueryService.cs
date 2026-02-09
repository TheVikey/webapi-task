using webapi_task.Infrastructure;

namespace webapi_task.Applications.Services;
public interface IFileQueryService
{
    Task<IEnumerable<Result>> GetFilteredResultsAsync(
        string? fileName = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null,
        double? averageValueFrom = null,
        double? averageValueTo = null,
        double? averageExecutionTimeFrom = null,
        double? averageExecutionTimeTo = null);
        Task<IEnumerable<Value>> GetLastValuesAsync(string fileName);
}
