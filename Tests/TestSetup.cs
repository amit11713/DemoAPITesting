using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using DemoAPITesting.Clients;
using DemoAPITesting.Configurations;

namespace DemoAPITesting.Tests;

// This setup fixture configures dependency injection and shared services for all tests in the assembly.
// It ensures a single, reusable IServiceProvider is available for test classes.
[SetUpFixture]
public class TestSetup
{
    // Provides access to the DI service provider for all tests
    public static IServiceProvider ServiceProvider { get; private set; } = null!;
    private static ServiceProvider? _disposableProvider;

    /// <summary>
    /// Runs once before any tests in the assembly. Configures DI, loads configuration, and registers services.
    /// </summary>
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        // Load configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        // Register ApiSettings as singleton from configuration
        var apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>() ?? new ApiSettings();
        services.AddSingleton(apiSettings);
        // Configure logging and register API client
        services.ConfigureLogging(configuration);
        services.AddScoped<IRestfulBookerClient, RestfulBookerClient>();
        // Build the service provider and assign it for global use
        _disposableProvider = services.BuildServiceProvider() as ServiceProvider;
        ServiceProvider = _disposableProvider;
    }

    /// <summary>
    /// Runs once after all tests in the assembly. Disposes the DI service provider.
    /// </summary>
    [OneTimeTearDown]
    public void Cleanup()
    {
        _disposableProvider?.Dispose();
    }
}
