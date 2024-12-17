using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Web.Filters.Action;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ValidateHeaderActionFilterAttribute(string headerKey) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.ContainsKey(headerKey))
        {
            context.Result = new BadRequestObjectResult($"Missing header {headerKey}");
        }
        else
        {
            await next();
        }
    }
}