using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RaftLabs.ConsoleApp.Demo;
using RaftLabs.Infrastructure.Extensions;
using RaftLabs.Infrastructure.Serialization;
using System.Text.Json;

namespace RaftLabs.Demo.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args) =>
            // Build the host with DI, logging, and configuration support
            await Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                // Load appsettings.json
                config.SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // Enable console logging
                _ = services.AddLogging(builder =>
                {
                    builder.AddConsole();
                });

                // Register demo launcher implementation
                _ = services.AddHostedService<LaunchDemo>();

                // Register the console arguments to be accessible globally
                _ = services.AddSingleton(args);

                // Register the external component services
                _ = services.AddInfrastructure(context.Configuration);

                // Registers customized JSON property naming policy to consider `snake_case` (expected from external API) for `PascalCase`
                _ = services.Configure<JsonSerializerOptions>(_ =>
                {
                    _.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    _.PropertyNameCaseInsensitive = true;
                    _.WriteIndented = true;
                });
            }).Build().RunAsync();
    }
}
