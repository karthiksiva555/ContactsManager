using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Web.Filters.Action;

public class ResponseHeaderAddAction(ILogger<ResponseHeaderAddAction> logger, string headerKey, string headerValue, int order) : IActionFilter, IOrderedFilter
{
    public int Order { get; } = order;
    
    public void OnActionExecuting(ActionExecutingContext context)
    {
        
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        logger.LogInformation("Adding response header {headerKey}: {headerValue}", headerKey, headerValue);
        context.HttpContext.Response.Headers.Append(headerKey, headerValue);
    }

    
}