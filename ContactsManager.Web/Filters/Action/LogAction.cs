using ContactsManager.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Web.Filters.Action;

public class LogAction(ILogger<LogAction> logger) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var controllerName = context.ActionDescriptor.RouteValues["controller"];
        var actionName = context.ActionDescriptor.RouteValues["action"];
        
        logger.LogInformation("Executing the {actionName} action method in {controllerName}.", actionName, controllerName);

        // context.ActionArguments -> Access all the action method parameters
        // check if parameter searchBy exists and null, set default value
        if (context.ActionArguments.TryGetValue("searchBy", out var value) && string.IsNullOrEmpty(value?.ToString()))
        {
            context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var controllerName = context.ActionDescriptor.RouteValues["controller"];
        var actionName = context.ActionDescriptor.RouteValues["action"];
        logger.LogInformation("Executed the action method {actionName} in {controllerName}.", actionName, controllerName);
        
        // Set controller name as title in ViewData
        var controller = context.Controller as Controller;
        controller?.ViewData.Add("ControllerTitle", controllerName);
        
        // Access parameters and set them in ViewData
        var parameters = context.HttpContext.Items["arguments"] as IDictionary<string, object>;
        if (parameters?.ContainsKey("searchBy") ?? false)
        {
            controller?.ViewData.Add("searchBy", parameters["searchBy"]);
        }
    }
}