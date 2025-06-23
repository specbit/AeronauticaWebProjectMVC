using FlyTickets2025.web.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlyTickets2025.web.Data
{
    public class SeedDb
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedDb(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync(); // Ensure database and tables exist

            // 1. Create Roles if they don't exist
            string[] roleNames = { "Administrador", "Funcionário", "Cliente", "Anónimo" };
            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Create Administrator User if none exists
            var adminEmail = "nuno.goncalo.gomes@formandos.cinel.pt";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                // Create a new admin user
                adminUser = new ApplicationUser
                {
                    FirstName = "System",
                    LastName = "Admin",
                    UserName = "nuno.goncalo.gomes@formandos.cinel.pt",
                    Email = "nuno.goncalo.gomes@formandos.cinel.pt",
                    EmailConfirmed = true // Assume initial admin is confirmed for easy setup
                };

                var creationPassword = "123456";
                var result = await _userManager.CreateAsync(adminUser, creationPassword); // !!! Use a strong, temporary password
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Administrador");
                }
            }

            // 3. (Optional) Seed initial Cities and Aircraft (for testing)
            if (!_context.Cities.Any())
            {
                _context.Cities.AddRange(
                    new City { Name = "Lisboa", Country = "Portugal", FlagImagePath = "/images/flags/pt.png" },
                    new City { Name = "Porto", Country = "Portugal", FlagImagePath = "/images/flags/pt.png" },
                    new City { Name = "Madrid", Country = "Spain", FlagImagePath = "/images/flags/es.png" }
                );
                await _context.SaveChangesAsync();
            }

            if (!_context.Aircrafts.Any())
            {
                _context.Aircrafts.AddRange(
                    new Aircraft { Model = "Boeing 737", TotalSeats = 180, PhotoPath = "/images/aircraft/b737.jpg" },
                    new Aircraft { Model = "Airbus A320", TotalSeats = 150, PhotoPath = "/images/aircraft/a320.jpg" }
                );
                await _context.SaveChangesAsync();
            }
        }
    }
}
