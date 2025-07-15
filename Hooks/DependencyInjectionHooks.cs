using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reqnroll;
using DemoAPITesting.Tests;

namespace DemoAPITesting.Hooks;

/// <summary>
/// Hooks for managing service scopes and dependency injection in Reqnroll scenarios.
/// Maintains the same service scoping pattern as existing NUnit tests for thread safety.
/// </summary>
[Binding]
public class DependencyInjectionHooks
{
    private IServiceScope? _scope;
    private readonly ScenarioContext _scenarioContext;

    public DependencyInjectionHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    /// <summary>
    /// Creates a service scope before each scenario to ensure thread safety and isolation.
    /// Follows the same pattern as NUnit tests in TestSetup.
    /// </summary>
    [BeforeScenario]
    public void BeforeScenario()
    {
        // Ensure TestSetup is initialized if not already done
        if (TestSetup.ServiceProvider == null)
        {
            // Initialize TestSetup manually for Reqnroll scenarios
            var testSetup = new TestSetup();
            testSetup.RunBeforeAnyTests();
        }
        
        // Use the existing TestSetup.ServiceProvider instead of conflicting with Reqnroll's DI
        _scope = TestSetup.ServiceProvider.CreateScope();
        _scenarioContext.Set(_scope, "ServiceScope");
        
        var logger = _scope.ServiceProvider.GetRequiredService<ILogger<DependencyInjectionHooks>>();
        logger.LogInformation("{Class}.{Method}: BeforeScenario - Service scope created for scenario '{ScenarioTitle}'", 
            nameof(DependencyInjectionHooks), nameof(BeforeScenario), _scenarioContext.ScenarioInfo.Title);
    }

    /// <summary>
    /// Disposes the service scope after each scenario to clean up resources.
    /// </summary>
    [AfterScenario]
    public void AfterScenario()
    {
        var logger = _scope?.ServiceProvider.GetRequiredService<ILogger<DependencyInjectionHooks>>();
        logger?.LogInformation("{Class}.{Method}: AfterScenario - Disposing service scope for scenario '{ScenarioTitle}'", 
            nameof(DependencyInjectionHooks), nameof(AfterScenario), _scenarioContext.ScenarioInfo.Title);
        
        _scope?.Dispose();
        _scenarioContext.Remove("ServiceScope");
    }
}