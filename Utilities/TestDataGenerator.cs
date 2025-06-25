using Bogus;
using DemoAPITesting.Models;
using System.Globalization;

namespace DemoAPITesting.Utilities;

public static class TestDataGenerator
{
    private static readonly Faker<BookingDates> BookingDatesFaker = new Faker<BookingDates>()
        .CustomInstantiator(f =>
        {
            // Generate a random checkout date in the past (up to 30 days ago)
            var checkout = f.Date.Past(1, DateTime.Today.AddDays(-1));
            checkout = checkout.Date; // Set time to 00:00:00
            // Generate a checkin date before checkout (1-14 days before checkout)
            var checkin = checkout.AddDays(-f.Random.Number(1, 14));
            checkin = checkin.Date; // Set time to 00:00:00
            return new BookingDates
            {
                Checkin = checkin,
                Checkout = checkout
            };
        });

    private static readonly Faker<Booking> BookingFaker = new Faker<Booking>()
        .RuleFor(b => b.Firstname, f => f.Name.FirstName())
        .RuleFor(b => b.Lastname, f => f.Name.LastName())
        .RuleFor(b => b.TotalPrice, f => f.Random.Number(100, 1000))
        .RuleFor(b => b.DepositPaid, f => f.Random.Bool())
        .RuleFor(b => b.BookingDates, _ => BookingDatesFaker.Generate())
        .RuleFor(b => b.AdditionalNeeds, f => f.Random.ArrayElement(new[]
            { "Breakfast", "Airport Transfer", "Extra Towels", "Late Checkout", null }));

    public static Booking GenerateBooking()
    {
        return BookingFaker.Generate();
    }

    public static List<Booking> GenerateBookings(int count)
    {
        return BookingFaker.Generate(count);
    }
}
