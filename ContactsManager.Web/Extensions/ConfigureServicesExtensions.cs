using ContactsManager.Application.ServiceInterfaces;
using ContactsManager.Application.Services;
using ContactsManager.Infrastructure;
using ContactsManager.Infrastructure.DbContext;
using ContactsManager.Web.Filters.Action;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Web.Extensions;

public static class ConfigureServicesExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews(options =>
        {
            // options.Filters.Add<LogAction>();
            // options.Filters.Add<ResponseHeaderAddAction>();
            // var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderAddAction>>();
            // options.Filters.Add(new ResponseHeaderAddAction(logger, "X-App-Key", "app-global", 1));
        });

        services.AddHttpLogging(options =>
        {
            // Log all fields
            // options.LoggingFields = HttpLoggingFields.All;
            // Log only request properties and response headers
            // options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponseBody;

            options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
        });
        
        //DI Container
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddInfrastructure();

        services.AddDbContext<ContactsDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnectionString")));

        // To be able to call the filter with ServiceFilter instead of TypeFilter
        services.AddTransient<LogActionAsync>();
        services.AddTransient<AddHeaderActionFilter>();
        
        return services;
    }
}