using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using DemoAPITesting.Clients;
using DemoAPITesting.Models;
using DemoAPITesting.Utilities;
using System.Runtime.InteropServices;

namespace DemoAPITesting.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class BookingTests
{
    private IRestfulBookerClient _client = null!;
    private ILogger<BookingTests> _logger = null!;
    private string _authToken = string.Empty;
    private IServiceScope _scope = null!;

    [SetUp]
    public async Task Setup()
    {
        _scope = TestSetup.ServiceProvider.CreateScope();
        _client = _scope.ServiceProvider.GetRequiredService<IRestfulBookerClient>();
        _logger = _scope.ServiceProvider.GetRequiredService<ILogger<BookingTests>>();
        
        // Perform health check for each test to ensure API is available
        var isHealthy = await _client.HealthCheckAsync();
        Assert.That(isHealthy, Is.True, "API health check failed");
        
        // Get authentication token for each test to ensure thread safety
        var apiSettings = _scope.ServiceProvider.GetRequiredService<DemoAPITesting.Configurations.ApiSettings>();
        _authToken = await _client.CreateTokenAsync(apiSettings.Username, apiSettings.Password);
        Assert.That(_authToken, Is.Not.Empty, "Failed to create authentication token");
    }

    [TearDown]
    public void TearDown()
    {
        _scope?.Dispose();
    }

    /// <summary>
    /// Creates a new booking and verifies that a booking ID is returned.
    /// </summary>
    [Test]
    public async Task CreateBooking_ShouldReturnNewBookingWithId()
    {
        _logger.LogInformation("{Class}.{Method}: Test Started", nameof(BookingTests), nameof(CreateBooking_ShouldReturnNewBookingWithId));
        
        // Generate test data for this test
        var testBooking = TestDataGenerator.GenerateBooking();
        
        // Create booking and retrieve the booking Id
        var bookingId = await _client.CreateBookingAsync(testBooking);
        Assert.That(bookingId, Is.Not.Null, "Booking creation should return a booking ID (HTTP 200)");
        Assert.That(bookingId.Value, Is.GreaterThan(0), "Booking ID should be greater than 0");
        
        _logger.LogInformation("{Class}.{Method}: Successfully created booking with ID: {BookingId}", nameof(BookingTests), nameof(CreateBooking_ShouldReturnNewBookingWithId), bookingId.Value);
        
        // Clean up - delete the created booking
        var deleteResult = await _client.DeleteBookingAsync(bookingId.Value, _authToken);
        _logger.LogInformation("{Class}.{Method}: Cleanup - deleted booking {BookingId}: {DeleteResult}", nameof(BookingTests), nameof(CreateBooking_ShouldReturnNewBookingWithId), bookingId.Value, deleteResult);
        
        _logger.LogInformation("{Class}.{Method}: Test Ended", nameof(BookingTests), nameof(CreateBooking_ShouldReturnNewBookingWithId));
    }

    /// <summary>
    /// Creates a booking, retrieves it, and verifies that the booking details match the expected values.
    /// </summary>
    [Test]
    public async Task GetBooking_ShouldReturnCorrectBooking()
    {
        _logger.LogInformation("{Class}.{Method}: Test Started", nameof(BookingTests), nameof(GetBooking_ShouldReturnCorrectBooking));
        
        // Generate test data for this test
        var testBooking = TestDataGenerator.GenerateBooking();
        
        // Create a booking first
        var bookingId = await _client.CreateBookingAsync(testBooking);
        Assert.That(bookingId, Is.Not.Null, "Booking creation should return a booking ID");
        
        _logger.LogInformation("{Class}.{Method}: Created booking with ID: {BookingId}, now retrieving it", nameof(BookingTests), nameof(GetBooking_ShouldReturnCorrectBooking), bookingId.Value);
        
        // Retrieve the booking and verify details
        var retrievedBooking = await _client.GetBookingAsync(bookingId.Value);
        Assert.That(retrievedBooking, Is.Not.Null, "Retrieved booking should not be null");
        Assert.That(retrievedBooking.Firstname, Is.EqualTo(testBooking.Firstname), "First name should match");
        Assert.That(retrievedBooking.Lastname, Is.EqualTo(testBooking.Lastname), "Last name should match");
        Assert.That(retrievedBooking.TotalPrice, Is.EqualTo(testBooking.TotalPrice), "Total price should match");
        Assert.That(retrievedBooking.DepositPaid, Is.EqualTo(testBooking.DepositPaid), "Deposit paid status should match");
        Assert.That(retrievedBooking.BookingDates.Checkin.Date, Is.EqualTo(testBooking.BookingDates.Checkin.Date), "Check-in date should match");
        Assert.That(retrievedBooking.BookingDates.Checkout.Date, Is.EqualTo(testBooking.BookingDates.Checkout.Date), "Check-out date should match");
        Assert.That(retrievedBooking.AdditionalNeeds, Is.EqualTo(testBooking.AdditionalNeeds), "Additional needs should match");
        
        // Clean up - delete the created booking
        var deleteResult = await _client.DeleteBookingAsync(bookingId.Value, _authToken);
        _logger.LogInformation("{Class}.{Method}: Cleanup - deleted booking {BookingId}: {DeleteResult}", nameof(BookingTests), nameof(GetBooking_ShouldReturnCorrectBooking), bookingId.Value, deleteResult);
        
        _logger.LogInformation("{Class}.{Method}: Test Ended", nameof(BookingTests), nameof(GetBooking_ShouldReturnCorrectBooking));
    }
    
    /// <summary>
    /// Creates a booking, updates it with new data, and verifies that the update is successful and the details are updated.
    /// </summary>
    [Test]
    public async Task UpdateBooking_ShouldUpdateBookingSuccessfully()
    {
        _logger.LogInformation("{Class}.{Method}: Test Started", nameof(BookingTests), nameof(UpdateBooking_ShouldUpdateBookingSuccessfully));
        
        // Generate initial test data for this test
        var initialBooking = TestDataGenerator.GenerateBooking();
        
        // Create a booking first
        var bookingId = await _client.CreateBookingAsync(initialBooking);
        Assert.That(bookingId, Is.Not.Null, "Booking creation should return a booking ID");
        
        _logger.LogInformation("{Class}.{Method}: Created booking with ID: {BookingId}, now updating it", nameof(BookingTests), nameof(UpdateBooking_ShouldUpdateBookingSuccessfully), bookingId.Value);
        
        // Generate updated booking data
        var updatedBooking = TestDataGenerator.GenerateBooking();
        
        // Update the booking
        var updateResult = await _client.UpdateBookingAsync(bookingId.Value, updatedBooking, _authToken);
        Assert.That(updateResult, Is.True, "Update booking should return true");
        
        // Retrieve and verify the updated booking
        var retrievedBooking = await _client.GetBookingAsync(bookingId.Value);
        Assert.That(retrievedBooking, Is.Not.Null, "Retrieved booking should not be null");
        Assert.That(retrievedBooking.Firstname, Is.EqualTo(updatedBooking.Firstname), "First name should be updated");
        Assert.That(retrievedBooking.Lastname, Is.EqualTo(updatedBooking.Lastname), "Last name should be updated");
        Assert.That(retrievedBooking.TotalPrice, Is.EqualTo(updatedBooking.TotalPrice), "Total price should be updated");
        Assert.That(retrievedBooking.DepositPaid, Is.EqualTo(updatedBooking.DepositPaid), "Deposit paid status should be updated");
        Assert.That(retrievedBooking.BookingDates.Checkin.Date, Is.EqualTo(updatedBooking.BookingDates.Checkin.Date), "Check-in date should be updated");
        Assert.That(retrievedBooking.BookingDates.Checkout.Date, Is.EqualTo(updatedBooking.BookingDates.Checkout.Date), "Check-out date should be updated");
        Assert.That(retrievedBooking.AdditionalNeeds, Is.EqualTo(updatedBooking.AdditionalNeeds), "Additional needs should be updated");
        
        // Clean up - delete the created booking
        var deleteResult = await _client.DeleteBookingAsync(bookingId.Value, _authToken);
        _logger.LogInformation("{Class}.{Method}: Cleanup - deleted booking {BookingId}: {DeleteResult}", nameof(BookingTests), nameof(UpdateBooking_ShouldUpdateBookingSuccessfully), bookingId.Value, deleteResult);
        
        _logger.LogInformation("{Class}.{Method}: Test Ended", nameof(BookingTests), nameof(UpdateBooking_ShouldUpdateBookingSuccessfully));
    }

    /// <summary>
    /// Creates a booking, deletes it, and verifies that the booking is removed successfully.
    /// </summary>
    [Test]
    public async Task DeleteBooking_ShouldRemoveBooking()
    {
        _logger.LogInformation("{Class}.{Method}: Test Started", nameof(BookingTests), nameof(DeleteBooking_ShouldRemoveBooking));
        
        // Generate test data for this test
        var testBooking = TestDataGenerator.GenerateBooking();
        
        // Create a booking first
        var bookingId = await _client.CreateBookingAsync(testBooking);
        Assert.That(bookingId, Is.Not.Null, "Booking creation should return a booking ID");
        
        _logger.LogInformation("{Class}.{Method}: Created booking with ID: {BookingId}, now deleting it", nameof(BookingTests), nameof(DeleteBooking_ShouldRemoveBooking), bookingId.Value);
        
        // Delete the booking
        var deleteResult = await _client.DeleteBookingAsync(bookingId.Value, _authToken);
        Assert.That(deleteResult, Is.True, "Delete booking should return true");
        
        // Verify the booking is deleted
        var retrievedBooking = await _client.GetBookingAsync(bookingId.Value);
        Assert.That(retrievedBooking, Is.Null, "Booking should be deleted");
        
        _logger.LogInformation("{Class}.{Method}: Test Ended", nameof(BookingTests), nameof(DeleteBooking_ShouldRemoveBooking));
    }
    
}
