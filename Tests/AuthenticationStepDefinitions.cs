using DemoAPITesting.Clients;
using DemoAPITesting.Configurations;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Reqnroll;
using System;
using System.Threading.Tasks;

namespace DemoAPITesting.Tests.StepDefinitions
{
    [Binding]
    public class AuthenticationStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly IRestfulBookerClient _client;
        private readonly ILogger<AuthenticationStepDefinitions> _logger;
        private readonly ApiSettings _apiSettings;

        // Reqnroll and the DI plugin will automatically inject these services.
        public AuthenticationStepDefinitions(
            ScenarioContext scenarioContext,
            IRestfulBookerClient client,
            ILogger<AuthenticationStepDefinitions> logger,
            ApiSettings apiSettings)
        {
            _scenarioContext = scenarioContext;
            _client = client;
            _logger = logger;
            _apiSettings = apiSettings;
        }

        [Given("I have valid username and password")]
        public void GivenIHaveValidUsernameAndPassword()
        {
            _logger.LogInformation("Setting up valid credentials");
            _scenarioContext.Set(_apiSettings.Username, "Username");
            _scenarioContext.Set(_apiSettings.Password, "Password");
        }

        [Given("I have invalid username and password")]
        public void GivenIHaveInvalidUsernameAndPassword()
        {
            _logger.LogInformation("Setting up invalid credentials");
            _scenarioContext.Set("invalidUsername", "Username");
            _scenarioContext.Set("invalidPassword", "Password");
        }

        [When("I attempt to create an authentication token")]
        public async Task WhenIAttemptToCreateAnAuthenticationToken()
        {
            _logger.LogInformation("Attempting to create authentication token");
            try
            {
                string user = _scenarioContext.Get<string>("Username");
                string pass = _scenarioContext.Get<string>("Password");
                string token = await _client.CreateTokenAsync(user, pass);
                _scenarioContext.Set(token, "Token");
            }
            catch (Exception ex)
            {
                _scenarioContext.Set(ex);
            }
        }

        [Then("I should receive a valid non-empty token")]
        public void ThenIShouldReceiveAValidNonEmptyToken()
        {
            _logger.LogInformation("Verifying valid token");
            Assert.That(_scenarioContext.TryGetValue("Token", out string token), Is.True, "Token was not found in the context.");
            Assert.That(token, Is.Not.Null.And.Not.Empty, "Token should not be empty.");
        }

        [Then("I should receive a {string} error message")]
        public void ThenIShouldReceiveAErrorMessage(string expectedErrorMessage)
        {
            _logger.LogInformation("Verifying error message: {ErrorMessage}", expectedErrorMessage);
            Assert.That(_scenarioContext.TryGetValue(out Exception ex), Is.True, "An exception was expected, but not found in the context.");
            Assert.That(ex.Message, Does.Contain(expectedErrorMessage), $"Exception message should contain '{expectedErrorMessage}'.");
        }
    }
}