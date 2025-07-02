using FlyTickets2025.web.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace FlyTickets2025.web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
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
            base.OnModelCreating(modelBuilder); // Necessário para Identity

            // Flight -> OriginCity (muitos voos têm uma cidade de origem)
            modelBuilder.Entity<Flight>()
                .HasOne(f => f.OriginCity)
                .WithMany()
                .HasForeignKey(f => f.OriginCityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Flight -> DestinationCity (muitos voos têm uma cidade de destino)
            modelBuilder.Entity<Flight>()
                .HasOne(f => f.DestinationCity)
                .WithMany()
                .HasForeignKey(f => f.DestinationCityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Flight -> Aircraft (muitos voos para uma aeronave)
            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Aircraft)
                .WithMany(a => a.Flights)
                .HasForeignKey(f => f.AircraftId)
                .OnDelete(DeleteBehavior.Restrict);

            // Flight -> Seats (um voo tem muitos assentos)
            modelBuilder.Entity<Flight>()
                .HasMany(f => f.Seats)
                .WithOne(s => s.Flight)
                .HasForeignKey(s => s.FlightId)
                .OnDelete(DeleteBehavior.Restrict);

            // Flight -> Tickets (um voo tem muitos tickets)
            modelBuilder.Entity<Flight>()
                .HasMany(f => f.Tickets)
                .WithOne(t => t.Flight)
                .HasForeignKey(t => t.FlightId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ticket -> Seat (um ticket tem um assento)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Seat)
                .WithMany()
                .HasForeignKey(t => t.SeatId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Ticket -> ApplicationUser (um ticket pertence a um usuário)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.ApplicationUser)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
