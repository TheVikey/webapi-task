using System.Globalization;
using webapi_task.Infrastructure;

namespace webapi_task.Infrastructure
{
    public interface ICsvParserService
    {
        Task<IEnumerable<Value>> ParseAsync(Stream stream, string fileName);
    }

    public class CsvParserService : ICsvParserService
    {
        public async Task<IEnumerable<Value>> ParseAsync(Stream stream, string fileName)
        {
            var values = new List<Value>();

            using var reader = new StreamReader(stream);
            string? line;
            int lineNumber = 0;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                lineNumber++;

                // Пропускаем пустые строки
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var value = ParseLine(line, fileName, lineNumber);
                values.Add(value);
            }

            return values;
        }

        private Value ParseLine(string line, string fileName, int lineNumber)
        {
            var parts = line.Split(';');

            if (parts.Length != 3)
            {
                throw new FormatException($"Line {lineNumber}: Expected 3 columns, got {parts.Length}");
            }

            // Проверяем, что все значения не пустые
            if (string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]) || string.IsNullOrWhiteSpace(parts[2]))
            {
                throw new FormatException($"Line {lineNumber}: All three values must be present (Date, ExecutionTime, Value)");
            }

            try
            {
                // Парсим дату (формат: 2024-01-15T14-30-00.1234Z)
                var date = DateTime.ParseExact(
                    parts[0].Trim(),
                    "yyyy-MM-ddTHH-mm-ss.ffffZ",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

                // Парсим время выполнения
                var executionTime = double.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);

                // Парсим значение
                var measurementValue = double.Parse(parts[2].Trim(), CultureInfo.InvariantCulture);

                return new Value
                {
                    FileName = fileName,
                    Date = date,
                    ExecutionTime = executionTime,
                    MeasurementValue = measurementValue
                };
            }
            catch (FormatException ex)
            {
                throw new FormatException($"Line {lineNumber}: Invalid format. Expected: yyyy-MM-ddTHH-mm-ss.ffffZ. Error: {ex.Message}");
            }
        }
    }
}
