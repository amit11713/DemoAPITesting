using DemoAPITesting.Configurations;
using DemoAPITesting.Models;
using DemoAPITesting.Utilities;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Net;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DemoAPITesting.Clients;

public class RestfulBookerClient : IRestfulBookerClient
{
    private readonly RestClient _client;
    private readonly ILogger<RestfulBookerClient> _logger;
    private readonly ApiSettings _apiSettings;

    public RestfulBookerClient(ILogger<RestfulBookerClient> logger, ApiSettings apiSettings)
    {
        _logger = logger;
        _apiSettings = apiSettings;
        var options = new RestClientOptions(apiSettings.BaseUrl);
        _client = new RestClient(options);
    }

    //This implementation demonstrates robust error handling that accounts for both
    //HTTP-level errors and application-level error responses that come with a 200 status code.
    public async Task<string> CreateTokenAsync(string username, string password)
    {
        // Log the attempt and create the request
        _logger.LogInformation("{Class}.{Method}: Creating authentication token for user {Username}", nameof(RestfulBookerClient), nameof(CreateTokenAsync), username);
        var request = new RestRequest("/auth", Method.Post);
        request.AddJsonBody(new { username, password });

        // Apply retry policy and execute the request
        var policy = RetryPolicyFactory.CreateRetryPolicy(_apiSettings.MaxRetries, _apiSettings.RetryDelayInMilliseconds, _logger);
        var response = await policy.ExecuteAsync(() => _client.ExecuteAsync(request));

        // 1. HTTP Error Responses
        if (!response.IsSuccessful)
        {
            _logger.LogError("{Class}.{Method}: Failed to create token. Status code: {StatusCode}, Error: {ErrorMessage}", nameof(RestfulBookerClient), nameof(CreateTokenAsync), response.StatusCode, response.ErrorMessage);
            throw new Exception($"Failed to create token: {response.ErrorMessage}");
        }

        // 2. API - Level Error(Bad Credentials)
        // This handles the case where the API returns HTTP 200 but includes { "reason": "Bad credentials" } in the body.
        using var doc = JsonDocument.Parse(response.Content!);
        if (doc.RootElement.TryGetProperty("reason", out var reasonProp))
        {
            var reason = reasonProp.GetString();
            _logger.LogError("{Class}.{Method}: Failed to create token. Reason: {Reason}", nameof(RestfulBookerClient), nameof(CreateTokenAsync), reason);
            throw new Exception($"Failed to create token: {reason}");
        }

        //3. Successful Authentication
        if (doc.RootElement.TryGetProperty("token", out var tokenProp))
        {
            var token = tokenProp.GetString();
            _logger.LogInformation("{Class}.{Method}: Successfully created authentication token", nameof(RestfulBookerClient), nameof(CreateTokenAsync));
            return token ?? string.Empty;
        }

        //4. Unexpected Response Format
        _logger.LogError("{Class}.{Method}: Unexpected response from token endpoint: {Response}", nameof(RestfulBookerClient), nameof(CreateTokenAsync), response.Content);
        throw new Exception($"Unexpected response from token endpoint: {response.Content}");
    }

    public async Task<int?> CreateBookingAsync(Booking booking)
    {
        _logger.LogInformation("{Class}.{Method}: Creating new booking for {FirstName} {LastName}", nameof(RestfulBookerClient), nameof(CreateBookingAsync), booking.Firstname, booking.Lastname);
        var bookingPayload = new
        {
            firstname = booking.Firstname,
            lastname = booking.Lastname,
            totalprice = booking.TotalPrice,
            depositpaid = booking.DepositPaid,
            bookingdates = new
            {
                checkin = booking.BookingDates.Checkin,
                checkout = booking.BookingDates.Checkout
            },
            additionalneeds = booking.AdditionalNeeds
        };
        var bookingJson = JsonSerializer.Serialize(bookingPayload, new JsonSerializerOptions {
            WriteIndented = true,
            Converters = { new DateOnlyJsonConverter() }
        });
        _logger.LogInformation("{Class}.{Method}: Booking data being sent: {BookingJson}", nameof(RestfulBookerClient), nameof(CreateBookingAsync), bookingJson);
        var request = new RestRequest("/booking", Method.Post);
        request.AddJsonBody(bookingJson, ContentType.Json);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");
        var policy = RetryPolicyFactory.CreateRetryPolicy(_apiSettings.MaxRetries, _apiSettings.RetryDelayInMilliseconds, _logger);
        var response = await policy.ExecuteAsync(() => _client.ExecuteAsync(request));
        if (!response.IsSuccessful)
        {
            _logger.LogError("{Class}.{Method}: Failed to create booking. Status code: {StatusCode}, Error: {ErrorMessage}", nameof(RestfulBookerClient), nameof(CreateBookingAsync), response.StatusCode, response.ErrorMessage);
            return null;
        }
        var responseObject = JsonSerializer.Deserialize<CreateBookingResponse>(response.Content!);
        _logger.LogInformation("{Class}.{Method}: Successfully created booking with ID: {BookingId}", nameof(RestfulBookerClient), nameof(CreateBookingAsync), responseObject?.BookingId);
        return responseObject?.BookingId;
    }

    public async Task<Booking?> GetBookingAsync(int bookingId)
    {
        _logger.LogInformation("{Class}.{Method}: Getting booking with ID: {BookingId}", nameof(RestfulBookerClient), nameof(GetBookingAsync), bookingId);
        var request = new RestRequest($"/booking/{bookingId}", Method.Get);
        request.AddHeader("Accept", "application/json");
        var policy = RetryPolicyFactory.CreateRetryPolicy(_apiSettings.MaxRetries, _apiSettings.RetryDelayInMilliseconds, _logger);
        var response = await policy.ExecuteAsync(() => _client.ExecuteAsync(request));
        if (!response.IsSuccessful)
        {
            _logger.LogError("{Class}.{Method}: Failed to get booking {BookingId}. Status code: {StatusCode}, Error: {ErrorMessage}", nameof(RestfulBookerClient), nameof(GetBookingAsync), bookingId, response.StatusCode, response.ErrorMessage);
            return null;
        }
        var booking = JsonSerializer.Deserialize<Booking>(
            response.Content!,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new DateOnlyJsonConverter() }
            });
        _logger.LogInformation("{Class}.{Method}: Successfully retrieved booking {BookingId}", nameof(RestfulBookerClient), nameof(GetBookingAsync), bookingId);
        return booking;
    }

    public async Task<List<int>> GetBookingIdsAsync()
    {
        _logger.LogInformation("{Class}.{Method}: Getting all booking IDs", nameof(RestfulBookerClient), nameof(GetBookingIdsAsync));
        var request = new RestRequest("/booking", Method.Get);
        request.AddHeader("Accept", "application/json");
        var policy = RetryPolicyFactory.CreateRetryPolicy(_apiSettings.MaxRetries, _apiSettings.RetryDelayInMilliseconds, _logger);
        var response = await policy.ExecuteAsync(() => _client.ExecuteAsync(request));
        if (!response.IsSuccessful)
        {
            _logger.LogError("{Class}.{Method}: Failed to get booking IDs. Status code: {StatusCode}, Error: {ErrorMessage}", nameof(RestfulBookerClient), nameof(GetBookingIdsAsync), response.StatusCode, response.ErrorMessage);
            return new List<int>();
        }
        var bookingIds = JsonSerializer.Deserialize<List<BookingIdResponse>>(response.Content!);
        _logger.LogInformation("{Class}.{Method}: Successfully retrieved {Count} booking IDs", nameof(RestfulBookerClient), nameof(GetBookingIdsAsync), bookingIds?.Count ?? 0);
        return bookingIds?.Select(b => b.BookingId).ToList() ?? new List<int>();
    }

    public async Task<bool> UpdateBookingAsync(int bookingId, Booking booking, string token)
    {
        _logger.LogInformation("{Class}.{Method}: Updating booking {BookingId}", nameof(RestfulBookerClient), nameof(UpdateBookingAsync), bookingId);
        var bookingPayload = new
        {
            firstname = booking.Firstname,
            lastname = booking.Lastname,
            totalprice = booking.TotalPrice,
            depositpaid = booking.DepositPaid,
            bookingdates = new
            {
                checkin = booking.BookingDates.Checkin,
                checkout = booking.BookingDates.Checkout
            },
            additionalneeds = booking.AdditionalNeeds
        };
        var bookingJson = JsonSerializer.Serialize(bookingPayload, new JsonSerializerOptions {
            Converters = { new DateOnlyJsonConverter() }
        });
        _logger.LogInformation("{Class}.{Method}: Update booking data being sent: {BookingJson}", nameof(RestfulBookerClient), nameof(CreateBookingAsync), bookingJson);
        var request = new RestRequest($"/booking/{bookingId}", Method.Put);
        request.AddJsonBody(bookingJson, ContentType.Json);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Cookie", $"token={token}");
        var policy = RetryPolicyFactory.CreateRetryPolicy(_apiSettings.MaxRetries, _apiSettings.RetryDelayInMilliseconds, _logger);
        var response = await policy.ExecuteAsync(() => _client.ExecuteAsync(request));
        if (!response.IsSuccessful)
        {
            _logger.LogError("{Class}.{Method}: Failed to update booking {BookingId}. Status code: {StatusCode}, Error: {ErrorMessage}", nameof(RestfulBookerClient), nameof(UpdateBookingAsync), bookingId, response.StatusCode, response.ErrorMessage);
            return false;
        }
        _logger.LogInformation("{Class}.{Method}: Successfully updated booking {BookingId}", nameof(RestfulBookerClient), nameof(UpdateBookingAsync), bookingId);
        return true;
    }

    public async Task<bool> PartialUpdateBookingAsync(int bookingId, Booking partialBooking, string token)
    {
        _logger.LogInformation("{Class}.{Method}: Partially updating booking {BookingId}", nameof(RestfulBookerClient), nameof(PartialUpdateBookingAsync), bookingId);
        var request = new RestRequest($"/booking/{bookingId}", Method.Patch);
        request.AddJsonBody(partialBooking);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Cookie", $"token={token}");
        var policy = RetryPolicyFactory.CreateRetryPolicy(_apiSettings.MaxRetries, _apiSettings.RetryDelayInMilliseconds, _logger);
        var response = await policy.ExecuteAsync(() => _client.ExecuteAsync(request));
        if (!response.IsSuccessful)
        {
            _logger.LogError("{Class}.{Method}: Failed to partially update booking {BookingId}. Status code: {StatusCode}, Error: {ErrorMessage}", nameof(RestfulBookerClient), nameof(PartialUpdateBookingAsync), bookingId, response.StatusCode, response.ErrorMessage);
            return false;
        }
        _logger.LogInformation("{Class}.{Method}: Successfully partially updated booking {BookingId}", nameof(RestfulBookerClient), nameof(PartialUpdateBookingAsync), bookingId);
        return true;
    }

    public async Task<bool> DeleteBookingAsync(int bookingId, string token)
    {
        _logger.LogInformation("{Class}.{Method}: Deleting booking {BookingId}", nameof(RestfulBookerClient), nameof(DeleteBookingAsync), bookingId);
        var request = new RestRequest($"/booking/{bookingId}", Method.Delete);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Cookie", $"token={token}");
        var policy = RetryPolicyFactory.CreateRetryPolicy(_apiSettings.MaxRetries, _apiSettings.RetryDelayInMilliseconds, _logger);
        var response = await policy.ExecuteAsync(() => _client.ExecuteAsync(request));
        if (!response.IsSuccessful)
        {
            _logger.LogError("{Class}.{Method}: Failed to delete booking {BookingId}. Status code: {StatusCode}, Error: {ErrorMessage}", nameof(RestfulBookerClient), nameof(DeleteBookingAsync), bookingId, response.StatusCode, response.ErrorMessage);
            return false;
        }
        _logger.LogInformation("{Class}.{Method}: Successfully deleted booking {BookingId}", nameof(RestfulBookerClient), nameof(DeleteBookingAsync), bookingId);
        return true;
    }

    public async Task<bool> HealthCheckAsync()
    {
        _logger.LogInformation("{Class}.{Method}: Performing health check", nameof(RestfulBookerClient), nameof(HealthCheckAsync));
        var request = new RestRequest("/ping", Method.Get);
        var policy = RetryPolicyFactory.CreateRetryPolicy(_apiSettings.MaxRetries, _apiSettings.RetryDelayInMilliseconds, _logger);
        var response = await policy.ExecuteAsync(() => _client.ExecuteAsync(request));
        var isHealthy = response.IsSuccessful;
        if (isHealthy)
        {
            _logger.LogInformation("{Class}.{Method}: Health check successful", nameof(RestfulBookerClient), nameof(HealthCheckAsync));
        }
        else
        {
            _logger.LogError("{Class}.{Method}: Health check failed. Status code: {StatusCode}, Error: {ErrorMessage}", nameof(RestfulBookerClient), nameof(HealthCheckAsync), response.StatusCode, response.ErrorMessage);
        }
        return isHealthy;
    }

    private class CreateBookingResponse
    {
        [JsonPropertyName("bookingid")]
        public int BookingId { get; set; }
        [JsonPropertyName("booking")]
        public Booking Booking { get; set; } = new();
    }

    private class BookingIdResponse
    {
        public int BookingId { get; set; }
    }
}
