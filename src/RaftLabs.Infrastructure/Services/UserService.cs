using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RaftLabs.Application.Abstractions;
using RaftLabs.Application.DTOs.Common;
using RaftLabs.Application.DTOs.Users;
using RaftLabs.Domain.Interfaces;
using RaftLabs.Domain.Models;
using RaftLabs.Infrastructure.Constants;

namespace RaftLabs.Infrastructure.Services
{
    /// <summary>
    /// Service for managing user-related operations, including retrieving all users and users from an external API.
    /// </summary>
    /// <param name="externalApiClient">Client for making requests to the external API.</param>
    /// <param name="logger">Logger for logging information and errors.</param>
    /// <param name="memoryCache">Memory cache for caching user data to reduce API calls.</param>
    /// <param name="mapper">Mapper for mapping between DTOs and entities.</param>
    public class UserService(IExternalApiClient externalApiClient, ILogger<UserService> logger, IMemoryCache memoryCache, IMapper mapper) : IUserService
    {
        private readonly IExternalApiClient _externalApiClient = externalApiClient;
        private readonly ILogger<UserService> _logger = logger;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Gets a list of all users from the external API, caching the results for 10 minutes.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing a list of <see cref="User"/> entities.</returns>
        public async Task<List<User>> GetAllUsersAsync()
        {
            // Check if users are already cached and return them if available.
            if (_memoryCache.TryGetValue(CacheKeys.Users, out List<User>? cachedUsers) && cachedUsers != null && cachedUsers.Count != 0)
            {
                // Log the total number of users retrieved from cache.
                _logger.LogInformation("Total users retrieved from cache: {total_users_count}.", cachedUsers.Count);

                // Return the cached users.
                return cachedUsers;
            }

            // Use a default page number of 1 for the initial request.
            int pageNumber = 1;

            // Log the start of the user retrieval process.
            _logger.LogInformation("Retrieving users from external API...");

            // Initialize an empty list to hold the users retrieved from the external API.
            List<User> users = [];

            try
            {
                // Loop to fetch users from the external API until all pages are retrieved.
                while (true)
                {
                    // Fetch users from the external API for the current page.
                    UsersResponseDto apiResponse = await _externalApiClient.GetUsersAsync(pageNumber);

                    // Add the users from the current page to the list only if there are any.
                    if (apiResponse.Data?.Any() ?? false)
                    {
                        // Log the number of users retrieved from the current page.
                        _logger.LogInformation("Users Retrieved: '{users_count}', Current page number: {page_number}, Total Pages: {total_pages}.", apiResponse.Data.Count(), apiResponse.Page, apiResponse.TotalPages);

                        // Map the UserDto objects to User entities and add them to the list.
                        users.AddRange(_mapper.Map<IEnumerable<UserDto>, IEnumerable<User>>(apiResponse.Data));
                    }
                    else
                    {
                        _logger.LogWarning("No users found in the API response for page {page_number}.", pageNumber);
                        break;
                    }

                    // Increment the page number for the next iteration.
                    pageNumber++;

                    // Break the loop if the page number exceeds the total number of pages.
                    if (pageNumber > apiResponse.TotalPages)
                        break;
                }

                // Cache the retrieved users for 10 minutes to avoid frequent API calls.
                _memoryCache.Set(CacheKeys.Users, users, TimeSpan.FromMinutes(10));

                // Log the total number of users retrieved.
                _logger.LogInformation("Total users retrieved and cached: {total_users_count}.", users.Count);

                // Return the list of users.
                return users;
            }
            catch (Exception ex)
            {
                // Log the error if the retrieval fails.
                _logger.LogError(ex, "Failed to retrieve users from external API.");

                // Throw the exception to indicate failure.
                throw;
            }
        }

        /// <summary>
        /// Gets the specified user by their ID from the cache or external API.
        /// </summary>
        /// <param name="userId">The ID of the specified user.</param>
        /// <returns>A task that represents the asynchronous operation, containing a <see cref="User"/> entity.</returns>
        public async Task<User> GetUserByIdAsync(int userId)
        {
            // Create a cache key for the user based on the user ID.
            string cacheKey = string.Format(CacheKeys.UserId, userId);

            // Check if the user is already cached.
            if (_memoryCache.TryGetValue(cacheKey, out User? cachedUser) && cachedUser != null)
            {
                // Log the user retrieval from cache.
                _logger.LogInformation("User with ID {user_id} retrieved from cache.", userId);

                // Return the cached user.
                return cachedUser;
            }

            try
            {
                // Log the start of the user retrieval process.
                _logger.LogInformation("Retrieving user with ID {user_id} from external API...", userId);

                // Fetch the user from the external API using the provided user ID.
                ApiResponseDto<UserDto> apiResponse = await _externalApiClient.GetUserByIdAsync(userId);

                // Map the UserDto to User entity.
                User userDetail = _mapper.Map<UserDto, User>(apiResponse.Data ?? new());

                // Cache the retrieved user for 10 minutes.
                _memoryCache.Set(cacheKey, userDetail, TimeSpan.FromMinutes(10));

                // Log that the user has been retrieved and cached.
                _logger.LogInformation("Retrieved and cached user with ID {user_id}.", userId);

                // Return the user retrieved from the external API.
                return userDetail;
            }
            catch (Exception ex)
            {
                // Log any errors that occur during retrieval.
                _logger.LogError(ex, "Failed to retrieve user with ID {user_id} from external API.", userId);

                // Throw the exception to indicate failure.
                throw;
            }
        }
    }
}
