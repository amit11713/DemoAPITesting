using System.Text.Json.Serialization;

namespace DemoAPITesting.Models;

public class Booking
{
    [JsonIgnore]
    public int? BookingId { get; set; }
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public int TotalPrice { get; set; }
    public bool DepositPaid { get; set; }
    public BookingDates BookingDates { get; set; } = new();
    public string? AdditionalNeeds { get; set; }
}
