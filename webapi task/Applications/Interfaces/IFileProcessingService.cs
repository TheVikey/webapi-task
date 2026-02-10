namespace webapi_task.Applications.Interfaces
{
    public interface IFileProcessingService
    {
        Task<ProcessingResult> ProcessFileAsync(Stream fileStream, string fileName);
    }

    public class ProcessingResult
    {
        public bool IsSuccess { get; set; }
        public Guid? ResultId { get; set; }
        public IEnumerable<string> Errors { get; set; } = new List<string>();

        public static ProcessingResult Success(Guid resultId)
        {
            return new ProcessingResult
            {
                IsSuccess = true,
                ResultId = resultId,
                Errors = new List<string>()
            };
        }

        public static ProcessingResult Failure(IEnumerable<string> errors)
        {
            return new ProcessingResult
            {
                IsSuccess = false,
                Errors = errors
            };
        }
    }
}
