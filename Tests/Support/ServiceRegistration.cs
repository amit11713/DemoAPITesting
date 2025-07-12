using DemoAPITesting.Clients;
using DemoAPITesting.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;

namespace DemoAPITesting.Tests.Support
{
    // The [Binding] attribute has been removed from this static class.
    public static class ServiceRegistration
    {
        [ScenarioDependencies]
        public static IServiceCollection CreateServices()
        {
            var services = new ServiceCollection();

            // Load configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Register ApiSettings from configuration
            var apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>() ?? new ApiSettings();
            services.AddSingleton(apiSettings);

            // Configure logging and register the API client
            services.ConfigureLogging(configuration);
            services.AddScoped<IRestfulBookerClient, RestfulBookerClient>();

            return services;
        }
    }
}