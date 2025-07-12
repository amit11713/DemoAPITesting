# DemoAPITesting

Comprehensive automated test suite for the Restful Booker API using .NET 8.0, NUnit, RestSharp, and Serilog with advanced parallel execution capabilities.

## Overview

This project contains automated tests for the [Restful Booker API](https://restful-booker.herokuapp.com/) with a focus on reliability, performance, and maintainability. The test suite demonstrates modern .NET testing practices including parallel execution, structured logging, dependency injection, and resilient HTTP communication.

## Key Features

- **Test Framework**: NUnit 4.2.2 with parallel execution support
- **HTTP Client**: RestSharp 110.2.0 for API communication
- **Resilience & Retry**: Polly 8.2.0 for automatic retry of transient failures
- **Test Data Generation**: Bogus 35.0.1 for generating realistic fake data
- **Logging**: Serilog 4.0.0 with structured logging and per-test log files
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection for service management
- **Parallel Execution**: Thread-safe test execution with proper resource isolation
- **Configuration Management**: JSON-based configuration with environment-specific settings

## Architecture

### Project Structure

```
DemoAPITesting/
├── Clients/                    # API client implementations
│   ├── IRestfulBookerClient.cs        # Client interface
│   └── RestfulBookerClient.cs         # Main API client with retry logic
├── Configurations/             # Configuration classes
│   ├── ApiSettings.cs                # API endpoint and auth settings
│   └── LoggingConfiguration.cs       # Serilog configuration
├── Models/                     # Data models
│   ├── Booking.cs                   # Main booking entity
│   ├── BookingDates.cs               # Date range model
│   └── TokenResponse.cs              # Authentication response
├── Tests/                      # Test implementations
│   ├── AuthTests.cs                  # Authentication tests
│   ├── BookingTests.cs               # CRUD operation tests
│   ├── LoggingTests.cs               # Logging verification tests
│   ├── ParallelExecutionTests.cs     # Parallel execution validation
│   ├── AuthenticationStepDefinitions.cs # BDD step definitions for authentication
│   ├── Hooks.cs                      # Reqnroll/NUnit hooks for test lifecycle
│   ├── Support/ServiceRegistration.cs # DI container setup for BDD scenarios
│   └── TestSetup.cs                  # DI container setup for NUnit tests
├── Utilities/                  # Helper classes
│   ├── DateOnlyJsonConverter.cs      # JSON serialization support
│   ├── RetryPolicyFactory.cs         # Polly retry policy factory
│   ├── TestDataGenerator.cs          # Bogus-based test data generation
│   └── TestNameEnricher.cs           # Serilog test name enrichment
├── Properties/                 # Project properties and configuration
│   ├── AssemblyInfo.cs                # NUnit parallel execution configuration
│   ├── NUnit.Runners.reqnroll.json    # Reqnroll NUnit runner config
│   └── reqnroll.json                  # Reqnroll runtime config
└──  Features/                   # BDD feature files and auto-generated code
    ├── Authentication.feature         # Gherkin feature file for authentication
    ├── Authentication.feature.cs      # Auto-generated code for authentication feature
    ├── Booking.feature                # Gherkin feature file for booking
    └── Booking.feature.cs             # Auto-generated code for booking feature
```

### Test Categories

- **Unit**: Isolated component tests (e.g., parallel execution validation)
- **Functional**: API integration tests (e.g., authentication, CRUD operations)

## Setup Instructions

### Prerequisites

- **.NET 8.0 SDK** or higher
- An IDE (Visual Studio 2022, Visual Studio Code, JetBrains Rider)
- Internet connection for API access

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd DemoAPITesting
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

### Configuration

The `appsettings.json` file contains all configuration settings:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://restful-booker.herokuapp.com",
    "Username": "admin",
    "Password": "password123",
    "MaxRetries": 3,
    "RetryDelayInMilliseconds": 1000
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    }
  }
}
```

**Configuration Options:**
- `BaseUrl`: Restful Booker API endpoint
- `Username/Password`: Authentication credentials
- `MaxRetries`: Number of retry attempts for failed requests
- `RetryDelayInMilliseconds`: Base delay between retries (exponential backoff)

## Running Tests

### Command Line

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run specific test category
dotnet test --filter Category=Unit
dotnet test --filter Category=Functional
```

### Visual Studio

1. Open **Test Explorer** (Test → Test Explorer)
2. Click **Run All** to execute all tests
3. Use filters to run specific test categories

### Expected Results

- **Unit Tests**: Should always pass (no external dependencies)
- **Functional Tests**: May fail if the external API is unavailable

## Parallel Execution

This project implements advanced parallel test execution for improved performance. See [PARALLEL_EXECUTION.md](PARALLEL_EXECUTION.md) for detailed information about:

- Assembly and fixture-level parallelization
- Thread-safe dependency injection
- Service scoping per test
- Authentication token isolation

## BDD Testing with Reqnroll

To enhance test clarity and collaboration, this project has adopted a Behavior-Driven Development (BDD) approach using [Reqnroll](https://reqnroll.net/). Traditional NUnit tests are being migrated to BDD scenarios written in Gherkin syntax, which are stored in `.feature` files. This approach provides living documentation that is understandable to both technical and non-technical stakeholders.

The BDD implementation also supports scenario-level parallel execution, significantly reducing test suite run times. For a comprehensive explanation of the BDD setup, parallel execution configuration, and thread-safe dependency injection for scenarios, please see [REQNROLL_PARALLEL_EXECUTION.md](REQNROLL_PARALLEL_EXECUTION.md).

## Logging

The project uses Serilog for comprehensive logging with the following features:

- **Per-test log files**: Each test writes to its own log file in the `Logs/` directory
- **Structured logging**: Consistent format with class and method names
- **Thread enrichment**: Logs include thread IDs for parallel execution debugging
- **Multiple sinks**: Console output and file-based logging

Log files are created as: `Logs/test-{TestName}.log`

## Troubleshooting

### Common Issues

1. **API Connection Failures**
   - The Restful Booker API (herokuapp.com) may be temporarily unavailable
   - Check API status at: https://restful-booker.herokuapp.com/ping
   - Functional tests will fail if the API is down

2. **Package Vulnerability Warnings**
   - RestSharp 110.2.0 has a known moderate severity vulnerability
   - This is a test project and the vulnerability doesn't affect functionality
   - Consider updating to a newer version if available

3. **Test Execution Timeouts**
   - Increase retry delay in `appsettings.json` if experiencing network issues
   - Check firewall settings for outbound HTTPS connections

### Debug Information

- Log files in `Logs/` directory contain detailed execution information
- Use `--verbosity normal` with `dotnet test` for detailed output
- Parallel execution information is logged in `ParallelExecutionTests`

## Contributing

When contributing to this project:

1. Follow the existing code style and patterns
2. Ensure all new tests have appropriate XML documentation
3. Use dependency injection for all services
4. Follow the Arrange-Act-Assert pattern for tests
5. Add structured logging with class and method names
6. Ensure thread safety for parallel execution compatibility

## Security Notes

- Default credentials are used for demonstration purposes
- No sensitive data should be committed to the repository
- API credentials are stored in `appsettings.json` for testing only

