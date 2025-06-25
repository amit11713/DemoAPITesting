# DemoAPITesting
Automated tests for the Restful Booker API using .NET 9, NUnit, RestSharp, and Serilog.

# Restful Booker API Test Automation

This project contains automated tests for the Restful Booker API (https://restful-booker.herokuapp.com/).

## Features

- **Test Framework**: NUnit for structuring and running tests
- **HTTP Client**: RestSharp for making API requests
- **Resilience & Retry**: Polly for automatic retry of transient failures
- **Test Data Generation**: Bogus for generating realistic fake data
- **Logging**: Serilog for comprehensive logging

## Project Structure

- **Tests**: Contains all test classes organized by API resource
- **Models**: Data models representing API resources
- **Clients**: API client for interacting with the Restful Booker API
- **Utilities**: Helper classes for test data generation and retry policies
- **Configurations**: Configuration classes and settings

## Setup Instructions

### Prerequisites

- .NET 9 SDK
- An IDE (e.g., Visual Studio 2022, Visual Studio Code, JetBrains Rider)

### Installation

1. Clone the repository
2. Open the solution in your IDE
3. Restore NuGet packages
dotnet restore
### Configuration

Modify the `appsettings.json` file to configure:

- API base URL
- Authentication credentials
- Retry policy settings
- Logging options

## Running Tests

### Using Visual Studio

1. Open the Test Explorer
2. Click "Run All" to execute all tests

### Using Command Line
dotnet test

