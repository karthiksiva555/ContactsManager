using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Web.Filters.Resource;

public class AppNameResourceFilter(ILogger<AppNameResourceFilter> logger) : IAsyncResourceFilter
{
    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        logger.LogInformation("{filterName}:{methodName} - starting", typeof(AppNameResourceFilter), nameof(OnResourceExecutionAsync));
        
        // Set x-app-name cookie before model binding
        context.HttpContext.Response.Cookies.Append("x-app-name", "ContactsManager");

        // Test IAlwaysRunResultFilter by short-circuiting
        // if (!context.HttpContext.Request.Headers.ContainsKey("x-app-name"))
        // {
        //     context.Result = new OkObjectResult("Short-Circuited in ResourceFilter");
        //     return;
        // }
        
        // Goes to next filters in filter pipeline (action, exception and result)
        await next();
        
        // after logic (comes here after result filter executes)
        logger.LogInformation("{filterName}:{methodName} - ending", typeof(AppNameResourceFilter), nameof(OnResourceExecutionAsync));
    }
}