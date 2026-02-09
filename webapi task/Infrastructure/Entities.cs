namespace webapi_task.Infrastructure;

public class Value
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public double ExecutionTime { get; set; }
    public double MeasurementValue { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Result
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public double TimeDeltaSeconds { get; set; }
    public DateTime FirstOperationStart { get; set; }
    public double AverageExecutionTime { get; set; }
    public double AverageValue { get; set; }
    public double MedianValue { get; set; }
    public double MaxValue { get; set; }
    public double MinValue { get; set; }
    public int RowCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}