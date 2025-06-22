namespace RaftLabs.Infrastructure.Configuration
{
    /// <summary>
    /// Used for configuring the resilience settings for HTTP client requests and responses.
    /// </summary>
    public class HttpResilienceSettings
    {
        public RetrySettings RetrySettings { get; set; } = new();

        public CircuitBreakerSettings CircuitBreakerSettings { get; set; } = new();

        public int ResponseTimeoutInMins { get; set; } = 10;
    }

    /// <summary>
    /// Retry policy configuration settings.
    /// </summary>
    public class RetrySettings
    {
        public int RetryAttemptCount { get; set; } = 3;

        public double ExponentialBaseDigit { get; set; } = 2;
    }

    /// <summary>
    /// Circuit breaker policy configuration settings.
    /// </summary>

    public class CircuitBreakerSettings
    {
        public int NumberOfEventsAllowedBeforeBreaking { get; set; } = 2;

        public double OpenCircuitDurationAllowedInSecs { get; set; } = 30;

    }
}
