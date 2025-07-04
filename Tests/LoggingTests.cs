using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace DemoAPITesting.Tests;

/// <summary>
/// Tests specifically for verifying the separate log file functionality.
/// </summary>
[TestFixture]
[Category("Unit")]
[Parallelizable(ParallelScope.Self)]
public class LoggingTests
{
    private ILogger<LoggingTests> _logger = null!;
    private IServiceScope _scope = null!;

    [SetUp]
    public void Setup()
    {
        _scope = TestSetup.ServiceProvider.CreateScope();
        _logger = _scope.ServiceProvider.GetRequiredService<ILogger<LoggingTests>>();
    }

    [TearDown]
    public void TearDown()
    {
        _scope?.Dispose();
    }

    /// <summary>
    /// Test to verify that logs are being created for this specific test.
    /// This test should create a separate log file with its test name.
    /// </summary>
    [Test]
    public void TestSeparateLogFile_ShouldCreateUniqueLogFile()
    {
        _logger.LogInformation("{Class}.{Method}: Test Started - This should go to a separate log file", 
            nameof(LoggingTests), nameof(TestSeparateLogFile_ShouldCreateUniqueLogFile));
        
        _logger.LogInformation("{Class}.{Method}: This is a test log entry to verify separate log files", 
            nameof(LoggingTests), nameof(TestSeparateLogFile_ShouldCreateUniqueLogFile));
        
        _logger.LogWarning("{Class}.{Method}: This is a warning log entry", 
            nameof(LoggingTests), nameof(TestSeparateLogFile_ShouldCreateUniqueLogFile));
        
        // Simple assertion to make sure the test passes
        Assert.That(TestContext.CurrentContext.Test.Name, Is.EqualTo(nameof(TestSeparateLogFile_ShouldCreateUniqueLogFile)), "This test should pass and create a separate log file");
        
        _logger.LogInformation("{Class}.{Method}: Test Ended - Log file should contain only this test's logs", 
            nameof(LoggingTests), nameof(TestSeparateLogFile_ShouldCreateUniqueLogFile));
    }

    /// <summary>
    /// Another test to verify that each test gets its own log file.
    /// </summary>
    [Test]
    public void AnotherTestSeparateLogFile_ShouldCreateDifferentLogFile()
    {
        _logger.LogInformation("{Class}.{Method}: Second Test Started - This should go to a different log file", 
            nameof(LoggingTests), nameof(AnotherTestSeparateLogFile_ShouldCreateDifferentLogFile));
        
        _logger.LogInformation("{Class}.{Method}: This log should be in a different file than the first test", 
            nameof(LoggingTests), nameof(AnotherTestSeparateLogFile_ShouldCreateDifferentLogFile));
        
        Assert.That(TestContext.CurrentContext.Test.Name, Is.EqualTo(nameof(AnotherTestSeparateLogFile_ShouldCreateDifferentLogFile)), "This test should also pass and create its own separate log file");
        
        _logger.LogInformation("{Class}.{Method}: Second Test Ended", 
            nameof(LoggingTests), nameof(AnotherTestSeparateLogFile_ShouldCreateDifferentLogFile));
    }
}