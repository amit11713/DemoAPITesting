using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using DemoAPITesting.Clients;
using DemoAPITesting.Models;
using DemoAPITesting.Utilities;
using System.Runtime.InteropServices;

namespace DemoAPITesting.Tests;

[TestFixture]
public class BookingTests
{
    private IRestfulBookerClient _client = null!;
    private ILogger<BookingTests> _logger = null!;
    private string _authToken = string.Empty;
    private Booking _testBooking = null!;
    private int _bookingId;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var serviceProvider = TestSetup.ServiceProvider;
        _client = serviceProvider.GetRequiredService<IRestfulBookerClient>();
        _logger = serviceProvider.GetRequiredService<ILogger<BookingTests>>();
        var isHealthy = await _client.HealthCheckAsync();
        Assert.That(isHealthy, Is.True, "API health check failed");
        var apiSettings = serviceProvider.GetRequiredService<DemoAPITesting.Configurations.ApiSettings>();
        _authToken = await _client.CreateTokenAsync(apiSettings.Username, apiSettings.Password);
        Assert.That(_authToken, Is.Not.Empty, "Failed to create authentication token");
    }

    [SetUp]
    public void Setup()
    {
        // Only generate a new booking if we don't already have one
        if (_testBooking == null)
        {
            _testBooking = TestDataGenerator.GenerateBooking();
        }
    }

    /// <summary>
    /// Creates a new booking and retrieves the booking Id.
    /// </summary>
    [Test, Order(1)]
    public async Task CreateBooking_ShouldReturnNewBookingWithId()
    {
        _logger.LogInformation("{Class}.{Method}: Test Started", nameof(BookingTests), nameof(CreateBooking_ShouldReturnNewBookingWithId));
        // 1. Create booking and retrive the booking Id
        var bookingId = await _client.CreateBookingAsync(_testBooking);
        Assert.That(bookingId, Is.Not.Null, "Booking creation should return a booking ID (HTTP 200)");
        _bookingId = bookingId.Value;
        _logger.LogInformation("{Class}.{Method}: Stored created booking with ID: {BookingId}", nameof(BookingTests), nameof(CreateBooking_ShouldReturnNewBookingWithId), _bookingId);
        _logger.LogInformation("{Class}.{Method}: Test Ended", nameof(BookingTests), nameof(CreateBooking_ShouldReturnNewBookingWithId));
    }

    /// <summary>
    /// Retrieves the previously created booking and verifies that the booking details match the expected values.
    /// </summary>
    [Test, Order(2)]
    public async Task GetBooking_ShouldReturnCorrectBooking()
    {
        _logger.LogInformation("{Class}.{Method}: Test Started", nameof(BookingTests), nameof(GetBooking_ShouldReturnCorrectBooking));
        Assert.That(_bookingId, Is.GreaterThan(0), "Booking ID should be set from previous test");
        _logger.LogInformation("{Class}.{Method}: Retrieving booking with ID: {BookingId}", nameof(BookingTests), nameof(GetBooking_ShouldReturnCorrectBooking), _bookingId);
        var booking = await _client.GetBookingAsync(_bookingId);
        Assert.That(booking, Is.Not.Null, "Retrieved booking should not be null");
        Assert.That(booking.Firstname, Is.EqualTo(_testBooking.Firstname), "First name should match");
        Assert.That(booking.Lastname, Is.EqualTo(_testBooking.Lastname), "Last name should match");
        Assert.That(booking.TotalPrice, Is.EqualTo(_testBooking.TotalPrice), "Total price should match");
        Assert.That(booking.DepositPaid, Is.EqualTo(_testBooking.DepositPaid), "Deposit paid status should match");
        Assert.That(booking.BookingDates.Checkin.Date, Is.EqualTo(_testBooking.BookingDates.Checkin.Date), "Check-in date should match");
        Assert.That(booking.BookingDates.Checkout.Date, Is.EqualTo(_testBooking.BookingDates.Checkout.Date), "Check-out date should match");
        Assert.That(booking.AdditionalNeeds, Is.EqualTo(_testBooking.AdditionalNeeds), "Additional needs should match");
        _logger.LogInformation("{Class}.{Method}: Test Ended", nameof(BookingTests), nameof(GetBooking_ShouldReturnCorrectBooking));
    }
    
    /// <summary>
    /// Updates the existing booking with new data and verifies that the update is successful and the details are updated.
    /// </summary>
    [Test, Order(3)]
    public async Task UpdateBooking_ShouldUpdateBookingSuccessfully()
    {
        _logger.LogInformation("{Class}.{Method}: Test Started", nameof(BookingTests), nameof(UpdateBooking_ShouldUpdateBookingSuccessfully));
        Assert.That(_bookingId, Is.GreaterThan(0), "Booking ID should be set from previous test");
        _logger.LogInformation("{Class}.{Method}: Updating booking details having ID: {BookingId}", nameof(BookingTests), nameof(UpdateBooking_ShouldUpdateBookingSuccessfully), _bookingId);
        var updatedBooking = TestDataGenerator.GenerateBooking();
        var updateResult = await _client.UpdateBookingAsync(_bookingId, updatedBooking, _authToken);
        Assert.That(updateResult, Is.True, "Update booking should return true");
        var retrievedBooking = await _client.GetBookingAsync(_bookingId);
        Assert.That(retrievedBooking, Is.Not.Null, "Retrieved booking should not be null");
        Assert.That(retrievedBooking.Firstname, Is.EqualTo(updatedBooking.Firstname), "First name should be updated");
        Assert.That(retrievedBooking.Lastname, Is.EqualTo(updatedBooking.Lastname), "Last name should be updated");
        Assert.That(retrievedBooking.TotalPrice, Is.EqualTo(updatedBooking.TotalPrice), "Total price should be updated");
        Assert.That(retrievedBooking.DepositPaid, Is.EqualTo(updatedBooking.DepositPaid), "Deposit paid status should be updated");
        Assert.That(retrievedBooking.BookingDates.Checkin.Date, Is.EqualTo(updatedBooking.BookingDates.Checkin.Date), "Check-in date should be updated");
        Assert.That(retrievedBooking.BookingDates.Checkout.Date, Is.EqualTo(updatedBooking.BookingDates.Checkout.Date), "Check-out date should be updated");
        Assert.That(retrievedBooking.AdditionalNeeds, Is.EqualTo(updatedBooking.AdditionalNeeds), "Additional needs should be updated");
        _testBooking = updatedBooking;
        _logger.LogInformation("{Class}.{Method}: Test Ended", nameof(BookingTests), nameof(UpdateBooking_ShouldUpdateBookingSuccessfully));
    }

    /// <summary>
    /// Deletes the existing booking and verifies that the booking is removed successfully.
    /// </summary>
    [Test, Order(4)]
    public async Task DeleteBooking_ShouldRemoveBooking()
    {
        _logger.LogInformation("{Class}.{Method}: Test Started", nameof(BookingTests), nameof(DeleteBooking_ShouldRemoveBooking));
        Assert.That(_bookingId, Is.GreaterThan(0), "Booking ID should be set from previous test");
        var deleteResult = await _client.DeleteBookingAsync(_bookingId, _authToken);
        Assert.That(deleteResult, Is.True, "Delete booking should return true");
        var retrievedBooking = await _client.GetBookingAsync(_bookingId);
        Assert.That(retrievedBooking, Is.Null, "Booking should be deleted");
        _logger.LogInformation("{Class}.{Method}: Test Ended", nameof(BookingTests), nameof(DeleteBooking_ShouldRemoveBooking));
    }
    
}
