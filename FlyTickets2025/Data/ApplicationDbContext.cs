using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlyTickets2025.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DO NOT add your custom DbSet properties (e.g., DbSet<City>) here yet.
    // You can add them later when you define those models.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Crucial for Identity table creation and configuration
                                            // No custom model configurations here yet
    }
}
