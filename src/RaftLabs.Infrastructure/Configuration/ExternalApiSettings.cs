namespace RaftLabs.Infrastructure.Configuration
{
    /// <summary>
    /// Settings for configuring the external API.
    /// </summary>
    public class ExternalApiSettings
    {
        /// <summary>
        /// Specifies the base URL for the external API.
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// Specifies the API key used for authenticating the external API
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Specifies the API value used for authenticating the external API
        /// </summary>
        public string ApiValue { get; set; } = string.Empty;
    }
}
