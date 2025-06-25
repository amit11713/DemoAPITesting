using DemoAPITesting.Models;

namespace DemoAPITesting.Clients;

public interface IRestfulBookerClient
{
    Task<string> CreateTokenAsync(string username, string password);
    Task<int?> CreateBookingAsync(Booking booking);
    Task<Booking?> GetBookingAsync(int bookingId);
    Task<List<int>> GetBookingIdsAsync();
    Task<bool> UpdateBookingAsync(int bookingId, Booking booking, string token);
    Task<bool> PartialUpdateBookingAsync(int bookingId, Booking partialBooking, string token);
    Task<bool> DeleteBookingAsync(int bookingId, string token);
    Task<bool> HealthCheckAsync();
}
