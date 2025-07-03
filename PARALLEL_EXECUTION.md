# Parallel Test Execution Configuration

This document explains the parallel test execution configuration implemented in the DemoAPITesting project.

## Overview

The project has been configured to support parallel test execution with thread safety to improve test performance. This implementation allows test fixtures to run in parallel while ensuring that shared resources are properly isolated.

## Key Components

### 1. Assembly-Level Configuration

**File:** `Properties/AssemblyInfo.cs`
- Enables parallel execution at the assembly level using `[Parallelizable(ParallelScope.Fixtures)]`
- Allows different test fixtures to run simultaneously in separate threads

### 2. Fixture-Level Configuration

**Files:** `Tests/AuthTests.cs`, `Tests/BookingTests.cs`, `Tests/ParallelExecutionTests.cs`
- Each test fixture is decorated with `[Parallelizable(ParallelScope.Self)]`
- Allows tests within the same fixture to run in parallel

### 3. Thread-Safe Dependency Injection

**File:** `Tests/TestSetup.cs`
- Changed `IRestfulBookerClient` registration from `AddSingleton` to `AddScoped`
- Ensures each test gets its own instance of the client, preventing thread conflicts

### 4. Test-Level Service Scoping

**Implementation Pattern:**
```csharp
private IServiceScope _scope = null!;

[SetUp]
public void Setup()
{
    _scope = TestSetup.ServiceProvider.CreateScope();
    _client = _scope.ServiceProvider.GetRequiredService<IRestfulBookerClient>();
    _logger = _scope.ServiceProvider.GetRequiredService<ILogger<TTestClass>>();
}

[TearDown]
public void TearDown()
{
    _scope?.Dispose();
}
```

### 5. Authentication Token Handling

**Thread-Safe Approach:**
- Authentication tokens are now fetched per test rather than shared across fixtures
- Each test gets its own authentication token, preventing race conditions
- BookingTests changed from `[OneTimeSetUp]` to `[SetUp]` for token management

## Benefits

1. **Improved Performance**: Tests can run in parallel, reducing overall execution time
2. **Thread Safety**: Each test gets isolated resources, preventing interference
3. **Better Resource Management**: Proper scope management ensures resources are cleaned up
4. **Maintainability**: Clear separation of concerns makes the codebase easier to maintain

## Verification

The `ParallelExecutionTests.cs` file contains tests that verify:
- Each test gets its own client instance
- Tests can run in parallel without conflicts
- Service scoping works correctly
- Thread safety is maintained

## Usage

To run tests in parallel:
```bash
dotnet test
```

The parallel execution will be enabled automatically due to the configuration attributes.

## Notes

- The API connectivity issues in the original tests are unrelated to the parallel execution implementation
- Tests that require API connectivity will still fail if the external service is unavailable
- The parallel execution tests demonstrate the thread safety implementation without requiring external dependencies