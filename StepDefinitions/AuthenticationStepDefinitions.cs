using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Reqnroll;
using DemoAPITesting.Clients;
using DemoAPITesting.Configurations;

namespace DemoAPITesting.StepDefinitions;

/// <summary>
/// Step definitions for authentication scenarios.
/// Maintains the same assertions and behavior as existing AuthTests.
/// </summary>
[Binding]
public class AuthenticationStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;
    private IRestfulBookerClient _client = null!;
    private ILogger<AuthenticationStepDefinitions> _logger = null!;
    private ApiSettings _apiSettings = null!;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _token = string.Empty;
    private Exception? _caughtException;

    public AuthenticationStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    private void InitializeServices()
    {
        if (_client == null)
        {
            var scope = _scenarioContext.Get<IServiceScope>("ServiceScope");
            _client = scope.ServiceProvider.GetRequiredService<IRestfulBookerClient>();
            _logger = scope.ServiceProvider.GetRequiredService<ILogger<AuthenticationStepDefinitions>>();
            _apiSettings = scope.ServiceProvider.GetRequiredService<ApiSettings>();
        }
    }

    [Given(@"I have valid API credentials")]
    public void GivenIHaveValidApiCredentials()
    {
        InitializeServices();
        _username = _apiSettings.Username;
        _password = _apiSettings.Password;
        _logger.LogInformation("{Class}.{Method}: Using valid credentials for user '{Username}'", 
            nameof(AuthenticationStepDefinitions), nameof(GivenIHaveValidApiCredentials), _username);
    }

    [Given(@"I have invalid API credentials with username ""([^""]*)"" and password ""([^""]*)""")]
    public void GivenIHaveInvalidApiCredentials(string username, string password)
    {
        InitializeServices();
        _username = username;
        _password = password;
        _logger.LogInformation("{Class}.{Method}: Using invalid credentials for user '{Username}'", 
            nameof(AuthenticationStepDefinitions), nameof(GivenIHaveInvalidApiCredentials), _username);
    }

    [When(@"I request an authentication token")]
    public async Task WhenIRequestAnAuthenticationToken()
    {
        InitializeServices();
        _logger.LogInformation("{Class}.{Method}: Requesting authentication token for user '{Username}'", 
            nameof(AuthenticationStepDefinitions), nameof(WhenIRequestAnAuthenticationToken), _username);
        
        try
        {
            _token = await _client.CreateTokenAsync(_username, _password);
            _logger.LogInformation("{Class}.{Method}: Successfully received token", 
                nameof(AuthenticationStepDefinitions), nameof(WhenIRequestAnAuthenticationToken));
        }
        catch (Exception ex)
        {
            _caughtException = ex;
            _logger.LogInformation("{Class}.{Method}: Caught exception: {Message}", 
                nameof(AuthenticationStepDefinitions), nameof(WhenIRequestAnAuthenticationToken), ex.Message);
        }
    }

    [Then(@"I should receive a valid token")]
    public void ThenIShouldReceiveAValidToken()
    {
        Assert.That(_caughtException, Is.Null, "No exception should be thrown for valid credentials");
        Assert.That(_token, Is.Not.Empty, "Token should not be empty");
        _logger.LogInformation("{Class}.{Method}: Token validation passed", 
            nameof(AuthenticationStepDefinitions), nameof(ThenIShouldReceiveAValidToken));
    }

    [Then(@"I should receive an authentication error")]
    public void ThenIShouldReceiveAnAuthenticationError()
    {
        Assert.That(_caughtException, Is.Not.Null, "An exception should be thrown for invalid credentials");
        _logger.LogInformation("{Class}.{Method}: Authentication error received as expected", 
            nameof(AuthenticationStepDefinitions), nameof(ThenIShouldReceiveAnAuthenticationError));
    }

    [Then(@"the error message should contain ""([^""]*)""")]
    public void ThenTheErrorMessageShouldContain(string expectedMessage)
    {
        Assert.That(_caughtException, Is.Not.Null, "An exception should be available to check the message");
        Assert.That(_caughtException.Message, Does.Contain(expectedMessage), 
            $"Exception should mention '{expectedMessage}'");
        _logger.LogInformation("{Class}.{Method}: Error message validation passed", 
            nameof(AuthenticationStepDefinitions), nameof(ThenTheErrorMessageShouldContain));
    }
}