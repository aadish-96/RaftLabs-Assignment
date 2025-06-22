using Microsoft.Extensions.Options;
using RaftLabs.Application.Abstractions;
using RaftLabs.Application.DTOs.Common;
using RaftLabs.Application.DTOs.Users;
using RaftLabs.Infrastructure.Configuration;
using RaftLabs.Infrastructure.Exceptions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaftLabs.External.Components.Clients
{
    /// <summary>
    /// Used to interact with an external API to retrieve user data.
    /// </summary>
    public class ExternalApiClient : IExternalApiClient
    {
        /// <summary>
        /// Provides methods to make HTTP requests to the external API.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Provides configuration settings for the external API, such as base URL and API authentication details.
        /// </summary>
        private readonly ExternalApiSettings _externalApiSettings;

        /// <summary>
        /// Provides JSON serialization options from the DI container.
        /// </summary>
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        /// <summary>
        /// Parameterized constructor for the <see cref="ExternalApiClient"/> class.
        /// </summary>
        /// <param name="httpClient">HTTP client instance obtained from DI container</param>
        /// <param name="externalApiSettings">Configurations of external API settings obtained from DI container</param>
        /// <param name="jsonSerializerOptions">JSON serializer options obtained from DI container</param>
        public ExternalApiClient(HttpClient httpClient, IOptions<ExternalApiSettings> externalApiSettings, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        {
            _httpClient = httpClient;
            _externalApiSettings = externalApiSettings.Value;
            _jsonSerializerOptions = jsonSerializerOptions.Value;
            _httpClient.BaseAddress = new Uri(_externalApiSettings.BaseUrl);
            _httpClient.DefaultRequestHeaders.Add(_externalApiSettings.ApiKey, _externalApiSettings.ApiValue);
        }

        /// <summary>
        /// Gets a list of users from the external API with pagination support.
        /// </summary>
        /// <param name="pageNumber">The page number of the users to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation, containing a <see cref="UsersResponseDto"/> with an <see cref="IEnumerable{UserDto}"/> of users (success) or throws an <see cref="ExternalServiceException"/> (failure).</returns>
        /// <exception cref="ExternalServiceException">Thrown when the external API call fails or the response cannot be deserialized.</exception>
        public async Task<UsersResponseDto> GetUsersAsync(int pageNumber)
        {
            try
            {
                // Get users from the external API with pagination support.
                HttpResponseMessage response = await _httpClient.GetAsync($"users?page={pageNumber}");

                // If the response is not successful, throw an exception.
                if (!response.IsSuccessStatusCode)
                    throw new ExternalServiceException($"Failed to retrieve users from the external API. Status code: {response.StatusCode}");

                // Deserialize the response content into UsersResponseDto.
                return await response.Content.ReadFromJsonAsync<UsersResponseDto>(options: _jsonSerializerOptions) ?? throw new ExternalServiceException("Failed to deserialize the response from the external API.");
            }
            catch (HttpRequestException ex)
            {
                throw new ExternalServiceException("An error occurred while communicating with the external API, possibly due to network issues or the service being unavailable.", ex);
            }
        }

        /// <summary>
        /// Gets the specified user by their ID from the external API.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve details for.</param>
        /// <returns>A task that represents the asynchronous operation, containing an <see cref="ApiResponseDto{UserDto}"/> of specified user (success) or throws a <see cref="UserNotFoundException"/> if the user is not found, or an <see cref="ExternalServiceException"/> for other failures.</returns>
        /// <exception cref="UserNotFoundException">Thrown when the user with the specified ID is not found in the external API.</exception>
        /// <exception cref="ExternalServiceException">Thrown when the external API call fails or the response cannot be deserialized.</exception>
        public async Task<ApiResponseDto<UserDto>> GetUserByIdAsync(int userId)
        {
            try
            {
                // Gets the user from the external API.
                HttpResponseMessage response = await _httpClient.GetAsync($"users/{userId}");

                // If the user is not found, throw a UserNotFoundException.
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new UserNotFoundException(userId);

                // If the response is not successful, throw an exception.
                if (!response.IsSuccessStatusCode)
                    throw new ExternalServiceException($"Failed to retrieve user details from the external API. Status code: {response.StatusCode}");

                // Deserialize the response content into ApiResponseDto<UserDto>.
                return await response.Content.ReadFromJsonAsync<ApiResponseDto<UserDto>>(options: _jsonSerializerOptions) ?? throw new ExternalServiceException("Failed to deserialize the response from the external API.");
            }
            catch (HttpRequestException ex)
            {
                throw new ExternalServiceException("An error occurred while communicating with the external API, possibly due to network issues or the service being unavailable.", ex);
            }
        }
    }
}
