using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FlyTickets2025.web.Models;

namespace FlyTickets2025.web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            if (User.IsInRole("Administrador") || User.IsInRole("Funcionário"))
            {
                // For Admins and Employees, show the dashboard view.
                return View();
            }
        }

        // For Clients and anonymous users, redirect to the public catalog.
        return RedirectToAction("HomeCatalog", "Flights");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
