using Microsoft.AspNetCore.Mvc;

namespace FlyTickets2025.web.Controllers
{
    public class ErrorHandlerController : Controller
    {
        [Route("ErrorHandler/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    // Return the specific view for 404 Not Found
                    ViewBag.ErrorMessage = "O recurso que procura não foi encontrado.";
                    return View("~/Views/Home/Error404.cshtml");
                case 403:
                    ViewBag.ErrorMessage = "Acesso Negado. Não tem permissão para aceder a este recurso.";
                    // Return the specific view for 403 Forbidden/Access Denied
                    return View("~/Views/Account/NotAuthorized.cshtml");
                case 500: // Added case for 500 
                    ViewBag.ErrorMessage = "Ocorreu um erro interno do servidor. Tente mais tarde";
                    return View("Error"); // Renders Views/ErrorHandler/Error.cshtml
                default:
                    // Fallback to a default (/Views/Shared/Error.cshtml (as a fallback))
                    return View("Error");
            }
        }

        // Dedicated action for 500 errors (unhandled exceptions).
        // It's the target of app.UseExceptionHandler().
        [HttpGet("Error")] // This defines the route for this action: /Error (or /ErrorHandler/Error)
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // TODO: Handle the error logging or any other logic you want to perform here.
            /* I'll leave this for future reference */
            // You can get details about the exception here if needed, for logging.
            // var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            // if (exceptionHandlerPathFeature?.Error != null)
            // {
            //     // Log the exception details (exceptionHandlerPathFeature.Error)
            //     // logger.LogError(exceptionHandlerPathFeature.Error, "Unhandled exception occurred.");
            // }

            // Retrieve the custom error message from TempData (set by SeatsController, for example)
            string errorMessage = TempData["CustomErrorMessage"] as string;

            // Provide a default generic message if no specific message was set
            ViewBag.ErrorMessage = errorMessage ?? "Ocorreu um erro inesperado.";

            // Return the custom Error view
            return View("Error"); // Renders Views/ErrorHandler/Error.cshtml
        }

        // For specific "Delete Restricted by FK" errors 
        [HttpGet("DeleteRestricted")] // Define a specific route: /DeleteRestricted (or /ErrorHandler/DeleteRestricted)
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult DeleteRestricted()
        {
            // Retrieve the specific message from TempData
            string specificMessage = TempData["SpecificErrorMessage"] as string;

            // Provide a default message if no specific one was set
            ViewBag.SpecificDeniedMessage = specificMessage ?? "Não foi possível realizar a operação de eliminação devido a restrições de dados.";

            // Return the NEW, SPECIFIC view for delete restrictions
            return View("~/Views/ErrorHandler/DeleteRestricted.cshtml"); // Renders Views/ErrorHandler/DeleteRestricted.cshtml
        }
    }
}
