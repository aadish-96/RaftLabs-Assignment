using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RaftLabs.Application.Abstractions;
using RaftLabs.Domain.Interfaces;
using RaftLabs.External.Components.Clients;
using RaftLabs.Infrastructure.Configuration;
using RaftLabs.Infrastructure.Mappings;
using RaftLabs.Infrastructure.Policies;
using RaftLabs.Infrastructure.Services;

namespace RaftLabs.Infrastructure.Extensions
{
    /// <summary>
    /// Handles the dependency injection for the external component services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the services of external components.
        /// </summary>
        /// <param name="services">Service collection in which external services are to be registered.</param>
        /// <param name="configuration">Host configuration instance</param>
        /// <returns>Updated service collection containing all the external services required.</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure strongly typed settings
            services.Configure<ExternalApiSettings>(configuration.GetSection("ExternalApiSettings"));

            // Register memory cache
            services.AddMemoryCache();

            // Register AutoMapper profile
            services.AddAutoMapper(typeof(MappingProfile));

            // Register HttpClient with retry and circuit breaker
            services.AddHttpClient<IExternalApiClient, ExternalApiClient>().AddHttpPolicyHandlers();

            // Register domain services
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
