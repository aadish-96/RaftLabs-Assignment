namespace RaftLabs.Infrastructure.Exceptions
{
    /// <summary>
    /// Used to indicate that an external service is unavailable or has encountered an error.
    /// </summary>
    /// <param name="message">The error message describing the issue.</param>
    /// <param name="inner">Optional inner exception for more details.</param>
    public class ExternalServiceException(string message, Exception? inner = null) : Exception(message, inner)
    {
    }
}
