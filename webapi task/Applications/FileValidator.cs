namespace webapi_task.Applications
{
    public interface IFileValidator
    {
        ValidationResult Validate(IEnumerable<Infrastructure.Value> values);
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public IEnumerable<string> Errors { get; set; } = new List<string>();
    }

    public class FileValidator : IFileValidator
    {
        private readonly int _maxRows = 10000;
        private readonly DateTime _minDate = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public ValidationResult Validate(IEnumerable<Infrastructure.Value> values)
        {
            var errors = new List<string>();
            var valueList = values.ToList();

            // Проверка количества строк
            if (valueList.Count < 1)
            {
                errors.Add("File must contain at least 1 row");
            }

            if (valueList.Count > _maxRows)
            {
                errors.Add($"File cannot contain more than {_maxRows} rows");
            }

            var now = DateTime.UtcNow;

            // Проверка каждой строки
            for (int i = 0; i < valueList.Count; i++)
            {
                var value = valueList[i];
                var rowNumber = i + 1;

                // Дата не может быть позже текущей
                if (value.Date > now)
                {
                    errors.Add($"Row {rowNumber}: Date cannot be in the future");
                }

                // Дата не может быть раньше 01.01.2000
                if (value.Date < _minDate)
                {
                    errors.Add($"Row {rowNumber}: Date cannot be earlier than 01.01.2000");
                }

                // Время выполнения не может быть меньше 0
                if (value.ExecutionTime < 0)
                {
                    errors.Add($"Row {rowNumber}: Execution time cannot be negative");
                }

                // Значение показателя не может быть меньше 0
                if (value.MeasurementValue < 0)
                {
                    errors.Add($"Row {rowNumber}: Value cannot be negative");
                }
            }

            return new ValidationResult
            {
                IsValid = errors.Count == 0,
                Errors = errors
            };
        }
    }
}
