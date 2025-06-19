using FlyTickets2025.Web.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlyTickets2025.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    // Add DbSets for entities
    public DbSet<City> Cities { get; set; }
    public DbSet<Aircraft> Aircrafts { get; set; }
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Seat> Seats { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Crucial for Identity table creation and configuration
                                            // No custom model configurations here yet

        // Configure relationships to prevent cascading deletes as required 
        // All OnDelete(DeleteBehavior.Restrict) are used to prevent cascading deletes as per assignment

        // Flight to City (Origin)
        modelBuilder.Entity<Flight>()
            .HasOne(f => f.OriginCity)
            .WithMany(c => c.OriginFlights) // From City.cs
            .HasForeignKey(f => f.OriginCityId)
            .OnDelete(DeleteBehavior.Restrict); // Or .NoAction

        // Flight to City (Destination)
        modelBuilder.Entity<Flight>()
            .HasOne(f => f.DestinationCity)
            .WithMany(c => c.DestinationFlights) // From City.cs
            .HasForeignKey(f => f.DestinationCityId)
            .OnDelete(DeleteBehavior.Restrict); // Or .NoAction

        // Flight to Aircraft
        modelBuilder.Entity<Flight>()
            .HasOne(f => f.Aircraft)
            .WithMany(a => a.Flights) // From Aircraft.cs
            .HasForeignKey(f => f.AircraftId)
            .OnDelete(DeleteBehavior.Restrict); // Or .NoAction

        // Flight to Seat
        modelBuilder.Entity<Flight>()
            .HasMany(f => f.AvailableSeats) // From Flight.cs
            .WithOne(s => s.Flight)
            .HasForeignKey(s => s.FlightId) // From Seat.cs
            .OnDelete(DeleteBehavior.Restrict);

        // Ticket to Flight 
        modelBuilder.Entity<Flight>()
            .HasMany(f => f.Tickets) // From Flight.cs
            .WithOne(t => t.Flight)
            .HasForeignKey(t => t.FlightId) // From Ticket.cs
            .OnDelete(DeleteBehavior.Restrict);

        // ApplicationUser to Ticket
        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.Tickets)
            .WithOne(t => t.ApplicationUser)
            .HasForeignKey(t => t.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict); // Or .NoAction, depending on whether deleting a user should delete their tickets.

        // Ticket to Aircraft (Model Link)
        modelBuilder.Entity<Seat>()
            .HasOne(s => s.AircraftModel)
            .WithMany(a => a.SeatsClass)
            .HasForeignKey(s => s.AircraftId)
            .IsRequired() // Since Seat.AircraftId is 'required'
            .OnDelete(DeleteBehavior.Restrict); // If you're keeping the AircraftTemplate link)

        // Ticket to Seat (One-to-One or One-to-Zero..One: A Ticket has one Seat, a Seat can optionally have one Ticket)
        // The foreign key is on the Ticket entity (SeatId).
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Seat) // Ticket has one Seat
            .WithOne(s => s.Ticket) // Seat has one (optional) Ticket
            .HasForeignKey<Ticket>(t => t.SeatId) // The FK is on the Ticket entity (SeatId property)
            .IsRequired() 
            .OnDelete(DeleteBehavior.Restrict);

        // Optional: Add a unique index on Seat.TicketId if you want to strictly enforce
        // that a Seat can only be linked to one Ticket, and TicketId is nullable.
        // This helps EF Core understand the 1-to-1 nature when the FK is nullable.
        modelBuilder.Entity<Seat>()
            .HasIndex(s => s.TicketId)
            .IsUnique()
            .HasFilter("[TicketId] IS NOT NULL"); // Only enforce uniqueness for non-null TicketIds
    }
}
