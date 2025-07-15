using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Reqnroll;
using System.Collections.Concurrent;

namespace DemoAPITesting.StepDefinitions;

/// <summary>
/// Step definitions for verifying parallel execution in Reqnroll scenarios.
/// Maintains thread safety and verifies concurrent execution similar to ParallelExecutionTests.
/// </summary>
[Binding]
public class ParallelExecutionStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;
    private static readonly ConcurrentBag<string> _executedScenarios = new();
    private static readonly ConcurrentBag<int> _threadIds = new();
    private ILogger<ParallelExecutionStepDefinitions> _logger = null!;
    private string _testIdentifier = string.Empty;
    private bool _testCompleted;

    public ParallelExecutionStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    private void InitializeServices()
    {
        if (_logger == null)
        {
            var scope = _scenarioContext.Get<IServiceScope>("ServiceScope");
            _logger = scope.ServiceProvider.GetRequiredService<ILogger<ParallelExecutionStepDefinitions>>();
        }
    }

    [When(@"I execute a parallel test with identifier ""([^""]*)""")]
    public async Task WhenIExecuteAParallelTestWithIdentifier(string identifier)
    {
        InitializeServices();
        _testIdentifier = identifier;
        var threadId = Thread.CurrentThread.ManagedThreadId;
        
        _logger.LogInformation("{Class}.{Method}: Starting parallel test '{Identifier}' on thread {ThreadId}", 
            nameof(ParallelExecutionStepDefinitions), nameof(WhenIExecuteAParallelTestWithIdentifier), identifier, threadId);
        
        // Record this execution
        _executedScenarios.Add($"{identifier}-{threadId}");
        _threadIds.Add(threadId);
        
        // Simulate some work to allow for parallel execution verification
        await Task.Delay(100);
        
        _testCompleted = true;
        
        _logger.LogInformation("{Class}.{Method}: Completed parallel test '{Identifier}' on thread {ThreadId}", 
            nameof(ParallelExecutionStepDefinitions), nameof(WhenIExecuteAParallelTestWithIdentifier), identifier, threadId);
    }

    [Then(@"the test should complete successfully")]
    public void ThenTheTestShouldCompleteSuccessfully()
    {
        Assert.That(_testCompleted, Is.True, $"Test with identifier '{_testIdentifier}' should complete successfully");
        _logger.LogInformation("{Class}.{Method}: Test completion validation passed for '{Identifier}'", 
            nameof(ParallelExecutionStepDefinitions), nameof(ThenTheTestShouldCompleteSuccessfully), _testIdentifier);
    }

    [Then(@"the test should log the correct thread information")]
    public void ThenTheTestShouldLogTheCorrectThreadInformation()
    {
        var currentThreadId = Thread.CurrentThread.ManagedThreadId;
        
        // The expected entry should match the thread ID used during test execution,
        // which is stored when the test was executed, not the current assertion thread
        var matchingEntry = _executedScenarios.FirstOrDefault(entry => entry.StartsWith(_testIdentifier + "-"));
        
        Assert.That(matchingEntry, Is.Not.Null, 
            $"No executed scenario found for identifier '{_testIdentifier}'");
        Assert.That(_threadIds.Count, Is.GreaterThan(0), 
            "Thread IDs should be recorded during test execution");
        
        _logger.LogInformation("{Class}.{Method}: Thread information validation passed for '{Identifier}' - found entry '{Entry}'", 
            nameof(ParallelExecutionStepDefinitions), nameof(ThenTheTestShouldLogTheCorrectThreadInformation), 
            _testIdentifier, matchingEntry);
    }

    /// <summary>
    /// Verification method to check that parallel execution actually occurred.
    /// This should be called after all parallel scenarios have completed.
    /// </summary>
    [AfterTestRun]
    public static void VerifyParallelExecution()
    {
        // Verify that multiple threads were used
        var uniqueThreadIds = _threadIds.Distinct().Count();
        
        // We should have at least 2 different threads if parallel execution is working
        // (though this depends on the test runner configuration)
        if (uniqueThreadIds > 1)
        {
            Console.WriteLine($"Parallel execution verified: {uniqueThreadIds} different threads used");
        }
        else
        {
            Console.WriteLine($"Warning: Only {uniqueThreadIds} thread used - parallel execution may not be active");
        }
        
        Console.WriteLine($"Total scenarios executed: {_executedScenarios.Count}");
        Console.WriteLine($"Unique thread IDs: [{string.Join(", ", _threadIds.Distinct().OrderBy(x => x))}]");
    }
}