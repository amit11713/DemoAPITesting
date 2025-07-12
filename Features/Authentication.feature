@Authentication @parallel
Feature: Authentication
    As an API client
    I want to authenticate with the Restful Booker API
    So that I can perform authenticated operations

@parallel
Scenario: Successful authentication with valid credentials
    Given I have valid username and password
    When I attempt to create an authentication token
    Then I should receive a valid non-empty token

@parallel
Scenario: Failed authentication with invalid credentials
    Given I have invalid username and password
    When I attempt to create an authentication token
    Then I should receive a "Bad credentials" error message
     