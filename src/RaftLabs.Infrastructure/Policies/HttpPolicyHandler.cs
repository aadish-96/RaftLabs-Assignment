using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
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
        /// <param name="builder">Http Client Builder Instance</param>
        /// <returns>Customized client builder instance</returns>
        public static IHttpClientBuilder AddHttpPolicyHandlers(this IHttpClientBuilder builder) =>
            // Retry policy for transient errors with exponential backoff
            builder
                .AddPolicyHandler((provider, request) => Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()  // Network-related failures
                .OrResult(msg => msg.StatusCode == HttpStatusCode.RequestTimeout || (int)msg.StatusCode >= 500) // Server-related errors
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        provider.GetRequiredService<ILogger<HttpClient>>().LogWarning("Retry {RetryAttempt} after {Delay}s due to: {Message}", retryAttempt, timespan.TotalSeconds, outcome.Exception?.Message);
                    }))

            // Timeout policy to cancel slow requests
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10)))

            // Circuit breaker policy to prevent overloading a failing endpoint
            .AddPolicyHandler((provider, request) =>
            {
                ILogger<HttpClient> logger = provider.GetRequiredService<ILogger<HttpClient>>();

                return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()  // Network-related failures
                .OrResult(msg => msg.StatusCode == HttpStatusCode.RequestTimeout || (int)msg.StatusCode >= 500) // Server-related errors
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (result, breakDelay) =>
                    {
                        logger.LogWarning("Circuit broken due to: {Message}. Retry after {Delay}s.", result.Exception?.Message, breakDelay.TotalSeconds);
                    },
                    onReset: () => logger.LogInformation("Circuit reset - calls are allowed again."),
                    onHalfOpen: () => logger.LogInformation("Circuit half-open - testing connection...")
                );
            })
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMinutes(10))); // Timeout policy to cancel slow requests
    }
}
