# GitHub Copilot Instructions for This Solution

## General Guidelines
- Always follow the existing coding style and conventions used in this repository.
- Use C# 13.0 and .NET 9 features where appropriate.
- Prefer explicit types over var unless type is obvious from the right-hand side.
- Use structured logging with class and method names at the beginning of every log statement.
- All public methods and classes should have XML documentation comments.
- Use dependency injection for all services and clients.
- Ensure all test code uses NUnit and follows the Arrange-Act-Assert pattern.
- For API tests, always check for both HTTP-level and application-level errors in responses.
- When handling API responses, check for error indicators in the response body (e.g., "reason" property) even if HTTP status is 200.
- Use `Assert.That` for all assertions in tests.
- When catching exceptions in tests, assert on the exception message content to ensure correct error handling.
- Use `ILogger<T>` for logging in all classes, and never use `Console.WriteLine` in production code or tests.
- All test data should be generated using the `TestDataGenerator` utility.
- All configuration should be loaded from `appsettings.json` via dependency injection.
- All new files must include the appropriate namespace and file-level documentation.

## File/Folder Structure
- Place all test classes in the `Tests` folder.
- Place all API clients in the `Clients` folder.
- Place all models in the `Models` folder.
- Place all utility/helper classes in the `Utilities` folder.
- Place all configuration classes in the `Configurations` folder.

## Security & Best Practices
- Never commit secrets or credentials to the repository.
- Use dependency injection for all external dependencies.
- Prefer immutable data structures where possible.
- Handle all exceptions gracefully and log errors with context.

---
_These instructions are to be followed by GitHub Copilot and all contributors for every change in this solution._