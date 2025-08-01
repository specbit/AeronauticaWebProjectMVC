using FlyTickets2025.web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlyTickets2025.web.Models;

public class SeatSearchViewModel
{
    public int? FlightId { get; set; }

    public SelectList Flights { get; set; }

    public IEnumerable<Seat> Seats { get; set; }
}