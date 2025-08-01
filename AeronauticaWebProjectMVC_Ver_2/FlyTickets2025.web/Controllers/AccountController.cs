using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.web.Helpers;
using FlyTickets2025.web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlyTickets2025.web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApplicationUserHelper _applicationUserHelper;

        public AccountController(IApplicationUserHelper userHelper)
        {
            _applicationUserHelper = userHelper;
        }

        // GET: Account/Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("HomeCatalog", "Flights");
            }

            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _applicationUserHelper.LoginAsync(model);

                if (result.Succeeded)
                {
                    // Get the user object by their email 
                    var user = await _applicationUserHelper.GetUserByEmailasync(model.Username); // Assuming Username is email

                    if (user == null)
                    {
                        this.ModelState.AddModelError(string.Empty, "User not found after successful login.");
                        return View(model);
                    }

                    // --- Role-based Redirection Logic ---

                    if (await _applicationUserHelper.IsUserInRoleAsync(user, "Administrador"))
                    {
                        return RedirectToAction("Index", "Home"); // Admin dashboard
                    }
                    else if (await _applicationUserHelper.IsUserInRoleAsync(user, "Funcionário"))
                    {
                        return RedirectToAction("Index", "Home"); // Employee dashboard
                    }
                    else if (await _applicationUserHelper.IsUserInRoleAsync(user, "Cliente"))
                    {
                        return RedirectToAction("HomeCatalog", "Flights"); // Client catalog
                    }

                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        var returnUrl = this.Request.Query["ReturnUrl"].First();

                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                    }

                    return RedirectToAction("Index", "Home"); // Generic home page
                }
            }

            this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return View(model);
        }

        // POST: Account/Logout => for safety reasons, we use POST for logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _applicationUserHelper.LogoutAsync();
            return RedirectToAction("HomeCatalog", "Flights");
        }

        // GET: Account/Register
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _applicationUserHelper.GetUserByEmailasync(model.Username);

                if (existingUser == null)
                {
                    existingUser = new ApplicationUser
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        UserName = model.Username,
                        Email = model.Username,
                    };
                }

                // Ensure the "Customer" role exists
                await _applicationUserHelper.CheckRoleAsync("Cliente");

                // Add the new user with the provided password to the database
                var result = await _applicationUserHelper.AddUserAsync(existingUser, model.Password);

                // Add the user to the "Customer" role
                await _applicationUserHelper.AddUserToRoleAsync(existingUser, "Cliente");


                // Check if the user was successfully created
                if (result != IdentityResult.Success)
                {
                    // If the user creation failed, add an error to the model state
                    ModelState.AddModelError(string.Empty, "User could not be created. Please check the details and try again.");
                    return View(model);
                }

                // If the user was created successfully, log them in
                var loginResult = await _applicationUserHelper.LoginAsync(new LoginViewModel
                {
                    Username = model.Username,
                    RememberMe = false,
                    Password = model.Password
                });

                // Check if the login was successful
                if (loginResult.Succeeded)
                {
                    // Redirect to the home page after successful registration and login
                    return RedirectToAction("Index", "Home");
                }

                // If the login failed, add an error to the model state
                ModelState.AddModelError(string.Empty, "User could not log in. Please check the details and try again.");
            }

            // If the model state is not valid or if there were errors, return the view with the model
            return View(model);
        }


        // GET: Account/ChangeUser
        public async Task<IActionResult> ChangeUser()
        {
            // Check if the user is authenticated
            var user = await _applicationUserHelper.GetUserByEmailasync(User.Identity.Name);
            var model = new ChangeUserViewModel();

            if (user != null)
            {
                // If the user is authenticated, return the view with the user's details
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;

            }
            return View(model);
        }

        // POST: Account/ChangeUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Get the current user by email
                var user = await _applicationUserHelper.GetUserByEmailasync(User.Identity.Name);

                if (user != null)
                {
                    // Update the user's details
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;

                    // Update the user in the database
                    var result = await _applicationUserHelper.UpdateUserAsync(user);
                    if (result.Succeeded)
                    {
                        // If the update was successful, redirect to the home page
                        ViewBag.UserMessage = "User details updated successfully.";
                        //return RedirectToAction("Index", "Home");
                    }

                    // If there were errors, add them to the model state
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            // If the model state is not valid or if there were errors, return the view with the model
            return View(model);
        }

        // GET: Account/ChangePassword
        public IActionResult ChangePassword()
        {
            // If the user is authenticated, return the ChangePassword view
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            // If the user is not authenticated, redirect to the login page
            return RedirectToAction("Login", "Account");
        }

        // POST: Account/ChangePassword
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Get the current user by email
                var user = await _applicationUserHelper.GetUserByEmailasync(User.Identity.Name);
                if (user != null)
                {
                    // Change the user's password
                    var result = await _applicationUserHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        // If the password change was successful, redirect to the ChangeUser view
                        ViewBag.UserMessage = "Password changed successfully."; //WOn't be seen as we redirect to ChangeUser
                        //return RedirectToAction("Index", "Home");
                        return RedirectToAction("ChangeUser");
                    }
                    // If there were errors, add them to the model state
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not Found");
                }
            }
            // If the model state is not valid or if there were errors, return the view with the model
            return View(model);
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        public IActionResult RegisterEmployee()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> RegisterEmployee(RegisterNewEmployeeViewModel model)
        {
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Check if the user already exists
                var existingUser = await _applicationUserHelper.GetUserByEmailasync(model.Username);

                // If the user does not exist, create a new user
                if (existingUser == null)
                {
                    existingUser = new ApplicationUser
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        UserName = model.Username,
                        Email = model.Username
                    };
                }

                // Ensure the "Customer" role exists
                await _applicationUserHelper.CheckRoleAsync("Funcionário");

                // Add the new user with the provided password to the database
                var result = await _applicationUserHelper.AddUserAsync(existingUser, model.Password);

                // Add the user to the "Customer" role
                await _applicationUserHelper.AddUserToRoleAsync(existingUser, "Funcionário");


                // Check if the user was successfully created
                if (result != IdentityResult.Success)
                {
                    // If the user creation failed, add an error to the model state
                    ModelState.AddModelError(string.Empty, "User could not be created. Please check the details and try again.");
                    return View(model);
                }

                // Redirect to the home page after successful registration and login
                return RedirectToAction("Index", "Home");
            }

            // If the model state is not valid or if there were errors, return the view with the model
            return View(model);
        }
    }
}
