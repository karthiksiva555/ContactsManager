using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Web.Controllers;

[Route("[controller]")]
public class ErrorController : Controller
{
    [Route("/Error")]
    public IActionResult Error()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionFeature is { Error: not null })
        {
            ViewBag.ErrorMessage = exceptionFeature.Error.Message;
        }
        return View();
    }

    [Route("500")]
    public IActionResult InternalServerError()
    {
        return View();
    }
}