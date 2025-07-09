# GitHub Copilot Instructions for This Solution

## General Guidelines
- Always follow the existing coding style and conventions used in this repository.
- Use .NET 8.0 features (project currently targets .NET 8.0, not .NET 9).
- Prefer explicit types over var unless type is obvious from the right-hand side.
- Use structured logging with class and method names at the beginning of every log statement.
- All public methods and classes should have XML documentation comments.
- Use dependency injection for all services and clients.
- Ensure all test code uses NUnit 4.2.2 and follows the Arrange-Act-Assert pattern.
- For API tests, always check for both HTTP-level and application-level errors in responses.
- When handling API responses, check for error indicators in the response body (e.g., "reason" property) even if HTTP status is 200.
- Use `Assert.That` for all assertions in tests.
- When catching exceptions in tests, assert on the exception message content to ensure correct error handling.
- Use `ILogger<T>` for logging in all classes, and never use `Console.WriteLine` in production code or tests.
- All test data should be generated using the `TestDataGenerator` utility class with Bogus.
- All configuration should be loaded from `appsettings.json` via dependency injection.
- All new files must include the appropriate namespace and file-level documentation.

## Parallel Execution Guidelines
- All test fixtures must include `[Parallelizable(ParallelScope.Self)]` attribute for thread safety.
- Use service scoping pattern: create `IServiceScope` in `[SetUp]` and dispose in `[TearDown]`.
- Never share state between tests - each test should be completely isolated.
- Authentication tokens must be created per test, not shared across fixtures.
- Use the established pattern for dependency injection in test classes.

## File/Folder Structure
- Place all test classes in the `Tests` folder.
- Place all API clients in the `Clients` folder.
- Place all models in the `Models` folder.
- Place all utility/helper classes in the `Utilities` folder.
- Place all configuration classes in the `Configurations` folder.
- Test categories: Use `[Category("Unit")]` for isolated tests, `[Category("Functional")]` for API integration tests.

## Logging Requirements
- Use Serilog for all logging with structured logging format.
- Include class and method names in every log statement: `{Class}.{Method}:` prefix.
- Log test start/end with appropriate information level.
- Use log enrichment for thread IDs and test names.
- Per-test log files are automatically created in the `Logs/` directory.

## Error Handling and Resilience
- Use Polly retry policies for all external API calls via `RetryPolicyFactory`.
- Handle transient failures with exponential backoff strategy.
- Always log retry attempts with appropriate context.
- Check for both HTTP status codes and application-level error indicators.

## Testing Best Practices
- Generate test data using `TestDataGenerator` utility and Bogus library.
- All tests must be thread-safe and support parallel execution.
- Use dependency injection for all external dependencies including HTTP clients.
- Verify both positive and negative test scenarios.
- Include appropriate XML documentation for all test methods.
- Use meaningful test method names that describe the scenario being tested.

## Security & Best Practices
- Never commit secrets or credentials to the repository.
- Use dependency injection for all external dependencies.
- Prefer immutable data structures where possible.
- Handle all exceptions gracefully and log errors with context.
- Be aware of known security vulnerabilities in dependencies (e.g., RestSharp 110.2.0).

## Package Management
- Current major dependencies and their versions:
  - .NET 8.0 (target framework)
  - NUnit 4.2.2 (test framework)
  - RestSharp 110.2.0 (HTTP client - has known vulnerability)
  - Polly 8.2.0 (resilience library)
  - Bogus 35.0.1 (test data generation)
  - Serilog 4.0.0 (logging framework)
  - Microsoft.Extensions.DependencyInjection 8.0.0

## Known Issues
- RestSharp 110.2.0 has a moderate severity vulnerability (GHSA-4rr6-2v9v-wcpc).
- External API (restful-booker.herokuapp.com) may be temporarily unavailable affecting functional tests.
- Parallel execution may show different behavior in CI environments vs local development.

---
_These instructions are to be followed by GitHub Copilot and all contributors for every change in this solution._