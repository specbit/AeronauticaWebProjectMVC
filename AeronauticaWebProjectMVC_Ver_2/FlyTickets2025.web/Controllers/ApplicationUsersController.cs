using FlyTickets2025.web.Helpers;
using FlyTickets2025.web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlyTickets2025.web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ApplicationUsersController : Controller
    {
        private readonly IApplicationUserHelper _applicationUserHelper;

        public ApplicationUsersController(IApplicationUserHelper applicationUserHelper)
        {
            _applicationUserHelper = applicationUserHelper;
        }

        // This action lists ONLY employees
        public async Task<IActionResult> ListEmployeesAsync()
        {
            var employees = await _applicationUserHelper.GetUsersInRoleAsync("Funcionário");
            return View(employees);
        }

        // This new action lists ALL users
        public async Task<IActionResult> AllUsersAsync()
        {
            var allUsers = await _applicationUserHelper.GetAllUsersAsync();

            var model = new List<ApplicationUserWithRolesViewModel>();

            foreach (var user in allUsers)
            {
                var roles = await _applicationUserHelper.GetUserRolesAsync(user);

                model.Add(new ApplicationUserWithRolesViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Roles = string.Join(", ", roles) // Join roles into a single string
                });
            }

            //model.OrderBy(u => u.UserName); // Sort by UserName
            //return View(model); // Return all users with roles sorted by UserName
            //return View(allUsers); // Return all users without filtering and no Role

            var sortedModel = model.OrderBy(u => OrderRoles.GetRoleSortOrder(u.Roles)).ToList();
            return View(sortedModel); // Return all users with roles sorted by role order
        }

        // This action lists ONLY clients
        public async Task<IActionResult> ListClientsAsync()
        {
            var clients = await _applicationUserHelper.GetUsersInRoleAsync("Cliente");
            return View(clients);
        }
    }
}
