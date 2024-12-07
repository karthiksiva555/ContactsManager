using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Web.Filters.Authorization;

public class SessionAuthorizationFilter(ILogger<SessionAuthorizationFilter> logger) : IAsyncAuthorizationFilter
{
    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        logger.LogInformation("{filterName}.{methodName} - verifying authorization requirements", nameof(SessionAuthorizationFilter), nameof(OnAuthorizationAsync));
        // Add Code to verify the authorization requirements
        
        // To test the IAlwaysRunResultFilter
        // context.Result = new ForbidResult("Not Authorized");
        
        return Task.CompletedTask;
    }
}