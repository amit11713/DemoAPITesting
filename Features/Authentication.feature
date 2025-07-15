Feature: Authentication
  In order to access protected resources
  As an API user
  I want to be able to authenticate with valid credentials and receive proper error messages for invalid credentials

  @Authentication @Functional
  Scenario: Authenticate with valid credentials
    Given I have valid API credentials
    When I request an authentication token
    Then I should receive a valid token

  @Authentication @Functional @Negative
  Scenario: Failed authentication with invalid credentials
    Given I have invalid API credentials with username "invalidUsername" and password "invalidPassword"
    When I request an authentication token
    Then I should receive an authentication error
    And the error message should contain "Bad credentials"