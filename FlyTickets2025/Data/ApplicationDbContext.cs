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

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DO NOT add your custom DbSet properties (e.g., DbSet<City>) here yet.
    // You can add them later when you define those models.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Crucial for Identity table creation and configuration
                                            // No custom model configurations here yet

        // Configure relationships to prevent cascading deletes as required 
        // For example, if a City is deleted, you probably don't want related Flights to be deleted.
        // You might need to adjust this depending on your desired behavior for deletion.
        modelBuilder.Entity<Flight>()
            .HasOne(f => f.OriginCity)
            .WithMany(c => c.OriginFlights) // Use the navigation property in City
            .HasForeignKey(f => f.OriginCityId)
            .OnDelete(DeleteBehavior.Restrict); // Or .NoAction

        modelBuilder.Entity<Flight>()
            .HasOne(f => f.DestinationCity)
            .WithMany(c => c.DestinationFlights) // Use the navigation property in City
            .HasForeignKey(f => f.DestinationCityId)
            .OnDelete(DeleteBehavior.Restrict); // Or .NoAction

        modelBuilder.Entity<Flight>()
            .HasOne(f => f.Aircraft)
            .WithMany(a => a.Flights) // Use the navigation property in Aircraft
            .HasForeignKey(f => f.AircraftId)
            .OnDelete(DeleteBehavior.Restrict); // Or .NoAction

        // Ensure ApplicationUser has a collection for Tickets
        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.Tickets)
            .WithOne(t => t.ApplicationUser)
            .HasForeignKey(t => t.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict); // Or .NoAction, depending on whether deleting a user should delete their tickets.
    }
}
