using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RaftLabs.Domain.Interfaces;
using RaftLabs.Domain.Models;
using RaftLabs.Infrastructure.Configuration;
using System.Text.Json;

namespace RaftLabs.ConsoleApp.Demo
{
    public class LaunchDemo(string[] args, ILogger<LaunchDemo> logger, IUserService userService, IOptions<ExternalApiSettings> externalApiSettings, IOptions<JsonSerializerOptions> jsonSerializerOptions) : IHostedService
    {
        private readonly string[] _args = args;
        private readonly ILogger<LaunchDemo> _logger = logger;
        private readonly IUserService _userService = userService;
        private readonly IOptions<ExternalApiSettings> externalApiSettings = externalApiSettings;
        private readonly IOptions<JsonSerializerOptions> _jsonSerializerOptions = jsonSerializerOptions;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Retrieves the user id from the arguments
                int userId = int.Parse(_args.First());

                // Fetch all the users if the argument provided is 0 otherwise fetch the user with the specified ID
                if (userId == 0)
                {
                    List<User> usersResponse = await _userService.GetAllUsersAsync();
                    _logger.LogInformation("Users: {UserListJson}", JsonSerializer.Serialize(value: usersResponse, options: _jsonSerializerOptions.Value));
                }
                else if (userId > 0)
                {
                    User user = await _userService.GetUserByIdAsync(userId);
                    _logger.LogInformation("User with ID {UserId}: {UserJson}", userId, JsonSerializer.Serialize(value: user, options: _jsonSerializerOptions.Value));
                }
                else
                    _logger.LogError("User Id must be non-negative integer. User Id received: '{userId}'", userId);
            }
            catch (Exception ex)
            {
                // Log any exceptions encountered during API interaction
                _logger.LogError(ex, "An error occurred while retrieving data from the API.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
