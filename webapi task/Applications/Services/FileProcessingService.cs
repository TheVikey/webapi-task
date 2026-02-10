using webapi_task.Applications.Interfaces;
using webapi_task.Infrastructure;
using webapi_task.Infrastructure.Interfaces;

namespace webapi_task.Applications.Services;
public class FileProcessingService : IFileProcessingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICsvParserService _csvParser;
    private readonly IFileValidator _validator;
    private readonly IStatisticsCalculator _calculator;

    public FileProcessingService(
        IUnitOfWork unitOfWork,
        ICsvParserService csvParser,
        IFileValidator validator,
        IStatisticsCalculator calculator)
    {
        _unitOfWork = unitOfWork;
        _csvParser = csvParser;
        _validator = validator;
        _calculator = calculator;
    }

    public async Task<ProcessingResult> ProcessFileAsync(Stream fileStream, string fileName)
    {
        try
        {
            // Начинаем транзакцию
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Парсим CSV файл
                var values = await _csvParser.ParseAsync(fileStream, fileName);

                // Валидируем данные
                var validationResult = _validator.Validate(values);
                if (!validationResult.IsValid)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ProcessingResult.Failure(validationResult.Errors);
                }

                // Только после успешной валидации удаляем существующие записи
                await DeleteExistingRecordsAsync(fileName);

                // Сохраняем значения в БД
                await _unitOfWork.Values.AddRangeAsync(values);

                // Вычисляем статистику
                var statistics = _calculator.CalculateStatistics(values);

                // Создаем и сохраняем результат
                var result = new Result
                {
                    FileName = fileName,
                    TimeDeltaSeconds = statistics.TimeDelta.TotalSeconds,
                    FirstOperationStart = statistics.MinDate,
                    AverageExecutionTime = statistics.AverageExecutionTime,
                    AverageValue = statistics.AverageValue,
                    MedianValue = statistics.MedianValue,
                    MaxValue = statistics.MaxValue,
                    MinValue = statistics.MinValue,
                    RowCount = statistics.RowCount
                };

                await _unitOfWork.Results.AddAsync(result);

                // Сохраняем изменения и коммитим транзакцию
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return ProcessingResult.Success(result.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ProcessingResult.Failure(new[] { ex.Message });
            }
        }
        catch (Exception ex)
        {
            return ProcessingResult.Failure(new[] { $"Transaction error: {ex.Message}" });
        }
    }

    private async Task DeleteExistingRecordsAsync(string fileName)
    {
        // Удаляем существующие записи (по требованию - перезаписываем)
        await _unitOfWork.Values.DeleteByFileNameAsync(fileName);
        await _unitOfWork.Results.DeleteByFileNameAsync(fileName);
    }
}
