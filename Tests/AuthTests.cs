using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using DemoAPITesting.Clients;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace DemoAPITesting.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class AuthTests
{
    private IRestfulBookerClient _client = null!;
    private ILogger<AuthTests> _logger = null!;
    private IServiceScope _scope = null!;

    [SetUp]
    public void Setup()
    {
        _scope = TestSetup.ServiceProvider.CreateScope();
        _client = _scope.ServiceProvider.GetRequiredService<IRestfulBookerClient>();
        _logger = _scope.ServiceProvider.GetRequiredService<ILogger<AuthTests>>();
    }

    [TearDown]
    public void TearDown()
    {
        _scope?.Dispose();
    }
    
    /// <summary>
    /// Verifies that a token is returned when valid credentials are provided.
    /// </summary>
    [Test]
    public async Task CreateToken_WithValidCredentials_ShouldReturnToken()
    {
        _logger.LogInformation("{Class}.{Method}: Test Started", nameof(AuthTests), nameof(CreateToken_WithValidCredentials_ShouldReturnToken));
        var apiSettings = _scope.ServiceProvider.GetRequiredService<DemoAPITesting.Configurations.ApiSettings>();
        var token = await _client.CreateTokenAsync(apiSettings.Username, apiSettings.Password);
        //Console.WriteLine($"Token received: {token}");
        Assert.That(token, Is.Not.Empty, "Token should not be empty");
        _logger.LogInformation("{Class}.{Method}: Test Ended", nameof(AuthTests), nameof(CreateToken_WithValidCredentials_ShouldReturnToken));
    }

    /// <summary>
    /// Verifies that an exception is thrown when invalid credentials are used to create a token.
    /// </summary>
    [Test]
    public async Task CreateToken_WithInvalidCredentials_ExpectException()
    {
        _logger.LogInformation("{Class}.{Method}: Test Started", nameof(AuthTests), nameof(CreateToken_WithInvalidCredentials_ExpectException));
        try
        {
            var token = await _client.CreateTokenAsync("invalidUsername", "invalidPassword");
            // If we get here, no exception was thrown and the test should fail
            Assert.Fail("Expected an exception due to invalid credentials, but none was thrown.");
        }
        catch (Exception ex)
        {
            _logger.LogInformation("{Class}.{Method}: Caught expected exception: {Message}", 
                nameof(AuthTests), nameof(CreateToken_WithInvalidCredentials_ExpectException), ex.Message);

            // Assert that the exception message contains the expected text
            Assert.That(ex.Message, Does.Contain("Bad credentials"),
                "Exception should mention 'Bad credentials'");
            
            // Test passes because we caught the expected exception
            Assert.Pass("Caught expected exception for invalid credentials");
        }
        _logger.LogInformation("{Class}.{Method}: Test Ended", nameof(AuthTests), nameof(CreateToken_WithInvalidCredentials_ExpectException));
    }
    
}
