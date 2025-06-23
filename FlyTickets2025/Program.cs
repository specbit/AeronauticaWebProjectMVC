using FlyTickets2025.Data;
using FlyTickets2025.Web.Data;
using FlyTickets2025.Web.Data.Entities;
using FlyTickets2025.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlyTickets2025;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Configure Entity Framework Core with SQL Server
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        // Configure Identity services
        //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireDigit = false; // relax password policy for testing
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
        })
            .AddRoles<IdentityRole>() // Using roles
            .AddEntityFrameworkStores<ApplicationDbContext>();

        // Add Razor Pages and MVC services
        builder.Services.AddControllersWithViews();

        // Register SeedDb as a scoped service
        builder.Services.AddScoped<SeedDb>();

        // Register the generic repository
        //builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // Register specific repositories
        builder.Services.AddScoped<ICityRepository, CityRepository>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        RunSeeding(app); // Call the seeding method after building the app

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        app.Run();
    }

    private static void RunSeeding(IHost host) // IHost is the base interface for WebApplication
    {
        var scopeFactory = host.Services.GetService<IServiceScopeFactory>();

        using (var scope = scopeFactory?.CreateScope())
        {
            // Resolve SeedDB from the service provider within this scope
            var seeder = scope?.ServiceProvider.GetService<SeedDb>();
            if (seeder != null)
            {
                seeder.SeedAsync().Wait(); // Call the async method and wait for it to complete
            }
            else
            {
                // Log an error if the seeder could not be resolved
                var logger = scope?.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError("Failed to retrieve SeedDB service during startup.");
            }
        }
    }
}
