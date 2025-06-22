using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using RaftLabs.Infrastructure.Configuration;
using System.Net;

namespace RaftLabs.Infrastructure.Policies
{
    /// <summary>
    /// Utility for building the customized handling of the Http requests and responses.
    /// </summary>
    public static class HttpPolicyHandler
    {
        /// <summary>
        /// Used for retrying or short-cicuiting against network-failures, or server-related errors.
        /// </summary>
        /// <param name="builder">Http Client Builder Instance.</param>
        /// <param name="configuration">Host configuration instance.</param>
        /// <returns>Customized client builder instance.</returns>
        public static IHttpClientBuilder AddHttpPolicyHandlers(this IHttpClientBuilder builder, IConfiguration configuration)
        {
            // Gets the Http resilience configurations
            HttpResilienceSettings httpResilienceSettings = configuration.GetSection("HttpResilienceSettings").Get<HttpResilienceSettings>() ?? new();

            // Retry policy for transient errors with exponential backoff
            return builder
            .AddPolicyHandler((provider, request) => Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()  // Network-related failures
            .OrResult(msg => msg.StatusCode == HttpStatusCode.RequestTimeout || (int)msg.StatusCode >= 500) // Server-related errors
            .WaitAndRetryAsync(
                retryCount: httpResilienceSettings.RetrySettings.RetryAttemptCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(httpResilienceSettings.RetrySettings.ExponentialBaseDigit, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    provider.GetRequiredService<ILogger<HttpClient>>().LogWarning("Retry {RetryAttempt} after {Delay}s due to: {Message}", retryAttempt, timespan.TotalSeconds, outcome.Exception?.Message);
                }))

            // Circuit breaker policy to prevent overloading a failing endpoint
            .AddPolicyHandler((provider, request) =>
            {
                ILogger<HttpClient> logger = provider.GetRequiredService<ILogger<HttpClient>>();

                return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()  // Network-related failures
                .OrResult(msg => msg.StatusCode == HttpStatusCode.RequestTimeout || (int)msg.StatusCode >= 500) // Server-related errors
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: httpResilienceSettings.CircuitBreakerSettings.NumberOfEventsAllowedBeforeBreaking,
                    durationOfBreak: TimeSpan.FromSeconds(httpResilienceSettings.CircuitBreakerSettings.OpenCircuitDurationAllowedInSecs),
                    onBreak: (result, breakDelay) =>
                    {
                        logger.LogWarning("Circuit broken due to: {Message}. Retry after {Delay}s.", result.Exception?.Message, breakDelay.TotalSeconds);
                    },
                    onReset: () => logger.LogInformation("Circuit reset - calls are allowed again."),
                    onHalfOpen: () => logger.LogInformation("Circuit half-open - testing connection...")
                );
            })
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMinutes(httpResilienceSettings.ResponseTimeoutInMins))); // Timeout policy to cancel slow requests
        }
    }
}
