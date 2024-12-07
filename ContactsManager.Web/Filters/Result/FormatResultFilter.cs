using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Web.Filters.Result;

public class FormatResultFilter(ILogger<FormatResultFilter> logger) : IAsyncAlwaysRunResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        logger.LogInformation("Executing {filterName}.{methodName} - starting", nameof(FormatResultFilter), nameof(OnResultExecutionAsync));
        
        await next();
        
        logger.LogInformation("Executing {filterName}.{methodName} - ending", nameof(FormatResultFilter), nameof(OnResultExecutionAsync));
    }
}