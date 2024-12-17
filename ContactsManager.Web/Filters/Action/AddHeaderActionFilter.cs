using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Web.Filters.Action;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AddHeaderActionFilterFactory(string header, string value, int order) : Attribute, IFilterFactory
{
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var filter = serviceProvider.GetRequiredService<AddHeaderActionFilter>();
        filter.HeaderKey = header;
        filter.HeaderValue = value;
        filter.Order = order;
        
        return filter;
    }

    public bool IsReusable { get; } = false;
}

public class AddHeaderActionFilter(ILogger<AddHeaderActionFilter> logger) : IAsyncActionFilter, IOrderedFilter
{
    public string HeaderKey { get; set; } = string.Empty;
    public string HeaderValue { get; set; } = string.Empty;
    public int Order { get; set; }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        logger.LogInformation("Add header - before action executed");
        await next();
        logger.LogInformation("Add header - after action executed");
        context.HttpContext.Response.Headers.Append(HeaderKey, HeaderValue);
    }
}