using System.Text.Json.Serialization;

namespace DemoAPITesting.Models;

public class BookingDates
{
    [JsonPropertyName("checkin")]
    public DateTime Checkin { get; set; }
    [JsonPropertyName("checkout")]
    public DateTime Checkout { get; set; }
}
