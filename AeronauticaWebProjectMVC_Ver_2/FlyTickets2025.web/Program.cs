using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FlyTickets2025.web.Repositories;
using FlyTickets2025.web.Helpers;
using FlyTickets2025.Web.Repositories;
using System.Net.Mail;

namespace FlyTickets2025.web
{
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

            // Register all your services here.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<SeedDb>();
            builder.Services.AddScoped<ICityRepository, CityRepository>();
            builder.Services.AddScoped<IAircraftRepository, AircraftRepository>();
            builder.Services.AddScoped<IFlightRepository, FlightRepository>();
            builder.Services.AddScoped<ISeatRepository, SeatRepository>();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();

            // Re-adding the services we worked on before in a stable way
            builder.Services.AddScoped<IConverterHelper, ConverterHelper>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IApplicationUserHelper, ApplicationUserHelper>();

            // Configure cookie and session settings
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/ErrorHandler/403";
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                // This will use the developer exception page.
                app.UseDeveloperExceptionPage();

                // This block is for creating the local email directory, which is correctly placed here.
                var smtpSettings = app.Configuration.GetSection("SmtpSettings");
                if (smtpSettings.GetValue<SmtpDeliveryMethod>("DeliveryMethod") == SmtpDeliveryMethod.SpecifiedPickupDirectory)
                {
                    var pickupDirectory = smtpSettings["PickupDirectoryLocation"];
                    if (!string.IsNullOrEmpty(pickupDirectory) && !Directory.Exists(pickupDirectory))
                    {
                        try
                        {
                            Directory.CreateDirectory(pickupDirectory);
                            Console.WriteLine($"Successfully created email pickup directory at: {pickupDirectory}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Warning: Failed to create email pickup directory at '{pickupDirectory}'. Error: {ex.Message}");
                        }
                    }
                }
            }

            RunSeeding(app);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStatusCodePagesWithReExecute("/ErrorHandler/{0}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Flights}/{action=HomeCatalog}/{id?}");

            app.Run();
        }

        private static void RunSeeding(IHost host)
        {
            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory?.CreateScope())
            {
                var seeder = scope?.ServiceProvider.GetService<SeedDb>();
                if (seeder != null)
                {
                    seeder.SeedAsync().Wait();
                }
                else
                {
                    var logger = scope?.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError("Failed to retrieve SeedDB service during startup.");
                }
            }
        }
    }
}
