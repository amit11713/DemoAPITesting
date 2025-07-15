using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Reqnroll;
using DemoAPITesting.Clients;
using DemoAPITesting.Configurations;
using DemoAPITesting.Models;
using DemoAPITesting.Utilities;

namespace DemoAPITesting.StepDefinitions;

/// <summary>
/// Step definitions for booking management scenarios.
/// Maintains the same assertions and behavior as existing BookingTests.
/// </summary>
[Binding]
public class BookingManagementStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;
    private IRestfulBookerClient _client = null!;
    private ILogger<BookingManagementStepDefinitions> _logger = null!;
    private string _authToken = string.Empty;
    private Booking _originalBooking = null!;
    private Booking _updatedBooking = null!;
    private Booking? _retrievedBooking;
    private int? _bookingId;
    private bool _operationResult;

    public BookingManagementStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    private void InitializeServices()
    {
        if (_client == null)
        {
            var scope = _scenarioContext.Get<IServiceScope>("ServiceScope");
            _client = scope.ServiceProvider.GetRequiredService<IRestfulBookerClient>();
            _logger = scope.ServiceProvider.GetRequiredService<ILogger<BookingManagementStepDefinitions>>();
        }
    }

    [Given(@"the API is healthy")]
    public async Task GivenTheApiIsHealthy()
    {
        InitializeServices();
        _logger.LogInformation("{Class}.{Method}: Performing API health check", 
            nameof(BookingManagementStepDefinitions), nameof(GivenTheApiIsHealthy));
        
        var isHealthy = await _client.HealthCheckAsync();
        Assert.That(isHealthy, Is.True, "API health check failed");
        
        _logger.LogInformation("{Class}.{Method}: API health check passed", 
            nameof(BookingManagementStepDefinitions), nameof(GivenTheApiIsHealthy));
    }

    [Given(@"I have a valid authentication token")]
    public async Task GivenIHaveAValidAuthenticationToken()
    {
        InitializeServices();
        var scope = _scenarioContext.Get<IServiceScope>("ServiceScope");
        var apiSettings = scope.ServiceProvider.GetRequiredService<ApiSettings>();
        
        _logger.LogInformation("{Class}.{Method}: Creating authentication token", 
            nameof(BookingManagementStepDefinitions), nameof(GivenIHaveAValidAuthenticationToken));
        
        _authToken = await _client.CreateTokenAsync(apiSettings.Username, apiSettings.Password);
        Assert.That(_authToken, Is.Not.Empty, "Failed to create authentication token");
        
        _logger.LogInformation("{Class}.{Method}: Authentication token created successfully", 
            nameof(BookingManagementStepDefinitions), nameof(GivenIHaveAValidAuthenticationToken));
    }

    [Given(@"I have valid booking details")]
    public void GivenIHaveValidBookingDetails()
    {
        _originalBooking = TestDataGenerator.GenerateBooking();
        _logger.LogInformation("{Class}.{Method}: Generated valid booking details", 
            nameof(BookingManagementStepDefinitions), nameof(GivenIHaveValidBookingDetails));
    }

    [Given(@"I have created a booking with valid details")]
    public async Task GivenIHaveCreatedABookingWithValidDetails()
    {
        InitializeServices();
        _originalBooking = TestDataGenerator.GenerateBooking();
        
        _logger.LogInformation("{Class}.{Method}: Creating booking for test setup", 
            nameof(BookingManagementStepDefinitions), nameof(GivenIHaveCreatedABookingWithValidDetails));
        
        _bookingId = await _client.CreateBookingAsync(_originalBooking);
        Assert.That(_bookingId, Is.Not.Null, "Booking creation should return a booking ID");
        
        _logger.LogInformation("{Class}.{Method}: Created booking with ID: {BookingId}", 
            nameof(BookingManagementStepDefinitions), nameof(GivenIHaveCreatedABookingWithValidDetails), _bookingId.Value);
    }

    [Given(@"I have updated booking details")]
    public void GivenIHaveUpdatedBookingDetails()
    {
        _updatedBooking = TestDataGenerator.GenerateBooking();
        _logger.LogInformation("{Class}.{Method}: Generated updated booking details", 
            nameof(BookingManagementStepDefinitions), nameof(GivenIHaveUpdatedBookingDetails));
    }

    [When(@"I create a booking")]
    public async Task WhenICreateABooking()
    {
        InitializeServices();
        _logger.LogInformation("{Class}.{Method}: Creating booking", 
            nameof(BookingManagementStepDefinitions), nameof(WhenICreateABooking));
        
        _bookingId = await _client.CreateBookingAsync(_originalBooking);
        
        _logger.LogInformation("{Class}.{Method}: Booking creation completed", 
            nameof(BookingManagementStepDefinitions), nameof(WhenICreateABooking));
    }

    [When(@"I retrieve the booking by its ID")]
    public async Task WhenIRetrieveTheBookingByItsId()
    {
        InitializeServices();
        Assert.That(_bookingId, Is.Not.Null, "Booking ID should be available for retrieval");
        
        _logger.LogInformation("{Class}.{Method}: Retrieving booking with ID: {BookingId}", 
            nameof(BookingManagementStepDefinitions), nameof(WhenIRetrieveTheBookingByItsId), _bookingId.Value);
        
        _retrievedBooking = await _client.GetBookingAsync(_bookingId.Value);
        
        _logger.LogInformation("{Class}.{Method}: Booking retrieval completed", 
            nameof(BookingManagementStepDefinitions), nameof(WhenIRetrieveTheBookingByItsId));
    }

    [When(@"I update the booking with new details")]
    public async Task WhenIUpdateTheBookingWithNewDetails()
    {
        InitializeServices();
        Assert.That(_bookingId, Is.Not.Null, "Booking ID should be available for update");
        
        _logger.LogInformation("{Class}.{Method}: Updating booking with ID: {BookingId}", 
            nameof(BookingManagementStepDefinitions), nameof(WhenIUpdateTheBookingWithNewDetails), _bookingId.Value);
        
        _operationResult = await _client.UpdateBookingAsync(_bookingId.Value, _updatedBooking, _authToken);
        
        _logger.LogInformation("{Class}.{Method}: Booking update completed", 
            nameof(BookingManagementStepDefinitions), nameof(WhenIUpdateTheBookingWithNewDetails));
    }

    [When(@"I delete the booking by its ID")]
    public async Task WhenIDeleteTheBookingByItsId()
    {
        InitializeServices();
        Assert.That(_bookingId, Is.Not.Null, "Booking ID should be available for deletion");
        
        _logger.LogInformation("{Class}.{Method}: Deleting booking with ID: {BookingId}", 
            nameof(BookingManagementStepDefinitions), nameof(WhenIDeleteTheBookingByItsId), _bookingId.Value);
        
        _operationResult = await _client.DeleteBookingAsync(_bookingId.Value, _authToken);
        
        _logger.LogInformation("{Class}.{Method}: Booking deletion completed", 
            nameof(BookingManagementStepDefinitions), nameof(WhenIDeleteTheBookingByItsId));
    }

    [Then(@"the API should return a new booking ID")]
    public void ThenTheApiShouldReturnANewBookingId()
    {
        Assert.That(_bookingId, Is.Not.Null, "Booking creation should return a booking ID (HTTP 200)");
        _logger.LogInformation("{Class}.{Method}: Booking ID validation passed", 
            nameof(BookingManagementStepDefinitions), nameof(ThenTheApiShouldReturnANewBookingId));
    }

    [Then(@"the booking ID should be greater than 0")]
    public void ThenTheBookingIdShouldBeGreaterThan()
    {
        Assert.That(_bookingId, Is.Not.Null, "Booking ID should not be null");
        Assert.That(_bookingId.Value, Is.GreaterThan(0), "Booking ID should be greater than 0");
        _logger.LogInformation("{Class}.{Method}: Booking ID value validation passed: {BookingId}", 
            nameof(BookingManagementStepDefinitions), nameof(ThenTheBookingIdShouldBeGreaterThan), _bookingId.Value);
    }

    [Then(@"the booking details should match the original booking")]
    public void ThenTheBookingDetailsShouldMatchTheOriginalBooking()
    {
        Assert.That(_retrievedBooking, Is.Not.Null, "Retrieved booking should not be null");
        Assert.That(_retrievedBooking.Firstname, Is.EqualTo(_originalBooking.Firstname), "First name should match");
        Assert.That(_retrievedBooking.Lastname, Is.EqualTo(_originalBooking.Lastname), "Last name should match");
        Assert.That(_retrievedBooking.TotalPrice, Is.EqualTo(_originalBooking.TotalPrice), "Total price should match");
        Assert.That(_retrievedBooking.DepositPaid, Is.EqualTo(_originalBooking.DepositPaid), "Deposit paid status should match");
        Assert.That(_retrievedBooking.BookingDates.Checkin.Date, Is.EqualTo(_originalBooking.BookingDates.Checkin.Date), "Check-in date should match");
        Assert.That(_retrievedBooking.BookingDates.Checkout.Date, Is.EqualTo(_originalBooking.BookingDates.Checkout.Date), "Check-out date should match");
        Assert.That(_retrievedBooking.AdditionalNeeds, Is.EqualTo(_originalBooking.AdditionalNeeds), "Additional needs should match");
        
        _logger.LogInformation("{Class}.{Method}: Booking details validation passed", 
            nameof(BookingManagementStepDefinitions), nameof(ThenTheBookingDetailsShouldMatchTheOriginalBooking));
    }

    [Then(@"the API should confirm the update was successful")]
    public void ThenTheApiShouldConfirmTheUpdateWasSuccessful()
    {
        Assert.That(_operationResult, Is.True, "Update booking should return true");
        _logger.LogInformation("{Class}.{Method}: Update operation confirmation validated", 
            nameof(BookingManagementStepDefinitions), nameof(ThenTheApiShouldConfirmTheUpdateWasSuccessful));
    }

    [Then(@"retrieving the booking should return the updated details")]
    public async Task ThenRetrievingTheBookingShouldReturnTheUpdatedDetails()
    {
        InitializeServices();
        Assert.That(_bookingId, Is.Not.Null, "Booking ID should be available for retrieval");
        
        _logger.LogInformation("{Class}.{Method}: Retrieving updated booking for validation", 
            nameof(BookingManagementStepDefinitions), nameof(ThenRetrievingTheBookingShouldReturnTheUpdatedDetails));
        
        var updatedRetrievedBooking = await _client.GetBookingAsync(_bookingId.Value);
        Assert.That(updatedRetrievedBooking, Is.Not.Null, "Retrieved booking should not be null");
        Assert.That(updatedRetrievedBooking.Firstname, Is.EqualTo(_updatedBooking.Firstname), "First name should be updated");
        Assert.That(updatedRetrievedBooking.Lastname, Is.EqualTo(_updatedBooking.Lastname), "Last name should be updated");
        Assert.That(updatedRetrievedBooking.TotalPrice, Is.EqualTo(_updatedBooking.TotalPrice), "Total price should be updated");
        Assert.That(updatedRetrievedBooking.DepositPaid, Is.EqualTo(_updatedBooking.DepositPaid), "Deposit paid status should be updated");
        Assert.That(updatedRetrievedBooking.BookingDates.Checkin.Date, Is.EqualTo(_updatedBooking.BookingDates.Checkin.Date), "Check-in date should be updated");
        Assert.That(updatedRetrievedBooking.BookingDates.Checkout.Date, Is.EqualTo(_updatedBooking.BookingDates.Checkout.Date), "Check-out date should be updated");
        Assert.That(updatedRetrievedBooking.AdditionalNeeds, Is.EqualTo(_updatedBooking.AdditionalNeeds), "Additional needs should be updated");
        
        _logger.LogInformation("{Class}.{Method}: Updated booking details validation passed", 
            nameof(BookingManagementStepDefinitions), nameof(ThenRetrievingTheBookingShouldReturnTheUpdatedDetails));
    }

    [Then(@"the API should confirm the booking was deleted")]
    public void ThenTheApiShouldConfirmTheBookingWasDeleted()
    {
        Assert.That(_operationResult, Is.True, "Delete booking should return true");
        _logger.LogInformation("{Class}.{Method}: Delete operation confirmation validated", 
            nameof(BookingManagementStepDefinitions), nameof(ThenTheApiShouldConfirmTheBookingWasDeleted));
    }

    [Then(@"retrieving the booking should return no result")]
    public async Task ThenRetrievingTheBookingShouldReturnNoResult()
    {
        InitializeServices();
        Assert.That(_bookingId, Is.Not.Null, "Booking ID should be available for retrieval");
        
        _logger.LogInformation("{Class}.{Method}: Verifying booking deletion by retrieval attempt", 
            nameof(BookingManagementStepDefinitions), nameof(ThenRetrievingTheBookingShouldReturnNoResult));
        
        var deletedBooking = await _client.GetBookingAsync(_bookingId.Value);
        Assert.That(deletedBooking, Is.Null, "Booking should be deleted");
        
        _logger.LogInformation("{Class}.{Method}: Booking deletion verification passed", 
            nameof(BookingManagementStepDefinitions), nameof(ThenRetrievingTheBookingShouldReturnNoResult));
    }

    /// <summary>
    /// Clean up any created bookings after scenarios to avoid leaving test data.
    /// Only cleans up if the booking still exists and we have authentication.
    /// </summary>
    [AfterScenario("@BookingManagement")]
    public async Task CleanupBooking()
    {
        if (_bookingId.HasValue && !string.IsNullOrEmpty(_authToken) && _client != null)
        {
            try
            {
                var existingBooking = await _client.GetBookingAsync(_bookingId.Value);
                if (existingBooking != null)
                {
                    await _client.DeleteBookingAsync(_bookingId.Value, _authToken);
                    _logger?.LogInformation("{Class}.{Method}: Cleanup - deleted booking {BookingId}", 
                        nameof(BookingManagementStepDefinitions), nameof(CleanupBooking), _bookingId.Value);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning("{Class}.{Method}: Cleanup failed for booking {BookingId}: {Error}", 
                    nameof(BookingManagementStepDefinitions), nameof(CleanupBooking), _bookingId.Value, ex.Message);
            }
        }
    }
}