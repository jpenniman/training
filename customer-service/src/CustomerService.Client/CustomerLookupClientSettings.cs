namespace Northwind.CustomerService.Client;

sealed class CustomerLookupClientSettings
{
    public const string SECTION_NAME = "CustomerLookup";
    
    public Uri Uri { get; set; } = new("https://localhost");
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(45); 
    public int MaxRetryAttempts { get; set; } = 3;
    public CircuitBreakerSettings CircuitBreakerSettings { get; set; } = new();
}

public sealed class CircuitBreakerSettings
{
    public TimeSpan BreakDuration { get; set; } = TimeSpan.FromMinutes(10);
    public double FailureThresholdPercentage { get; set; } = 0.5;
    public int MinRequestsDuringSamplingWindow { get; set; } = 10;
    public TimeSpan SamplingWindow { get; set; } = TimeSpan.FromMinutes(10);
}