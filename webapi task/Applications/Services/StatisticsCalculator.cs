using System.Linq;

namespace webapi_task.Applications.Services;
public interface IStatisticsCalculator
{
    Statistics CalculateStatistics(IEnumerable<Infrastructure.Value> values);
}

public class Statistics
{
    public TimeSpan TimeDelta { get; set; }
    public DateTime MinDate { get; set; }
    public double AverageExecutionTime { get; set; }
    public double AverageValue { get; set; }
    public double MedianValue { get; set; }
    public double MaxValue { get; set; }
    public double MinValue { get; set; }
    public int RowCount { get; set; }
}

public class StatisticsCalculator : IStatisticsCalculator
{
    public Statistics CalculateStatistics(IEnumerable<Infrastructure.Value> values)
    {
        var valueList = values.ToList();

        if (!valueList.Any())
        {
            throw new ArgumentException("Values collection cannot be empty");
        }

        // Сортируем значения для вычисления медианы
        var sortedValues = valueList
            .Select(v => v.MeasurementValue)
            .OrderBy(v => v)
            .ToList();

        var dates = valueList.Select(v => v.Date).ToList();
        var executionTimes = valueList.Select(v => v.ExecutionTime).ToList();

        return new Statistics
        {
            MinDate = dates.Min(),
            TimeDelta = dates.Max() - dates.Min(),
            AverageExecutionTime = executionTimes.Average(),
            AverageValue = sortedValues.Average(),
            MaxValue = sortedValues.Last(),
            MinValue = sortedValues.First(),
            RowCount = valueList.Count,
            MedianValue = CalculateMedian(sortedValues)
        };
    }

    private double CalculateMedian(List<double> sortedValues)
    {
        int count = sortedValues.Count;

        if (count == 0) return 0;

        if (count % 2 == 0)
        {
            // Для чётного количества два средних элемента
            int middleIndex = count / 2;
            return (sortedValues[middleIndex - 1] + sortedValues[middleIndex]) / 2.0;
        }
        else
        {
            // Для нечётного средний элемент
            return sortedValues[count / 2];
        }
    }
}
