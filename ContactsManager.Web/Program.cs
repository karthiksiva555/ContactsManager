using ContactsManager.Application.Interfaces;
using ContactsManager.Application.Services;
using ContactsManager.Core.Entities;
using ContactsManager.Infrastructure;
using ContactsManager.Web.Filters.Action;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure Logging providers
// builder.Logging.ClearProviders();
// builder.Logging.AddConsole();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<LogAction>();
    // options.Filters.Add<ResponseHeaderAddAction>();
    var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderAddAction>>();
    options.Filters.Add(new ResponseHeaderAddAction(logger, "X-App-Key", "app-global", 1));
});

builder.Services.AddHttpLogging(options =>
{
    // Log all fields
    // options.LoggingFields = HttpLoggingFields.All;
    // Log only request properties and response headers
    // options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponseBody;

    options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
});

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        // let Serilog access our config from IConfiguration
        .ReadFrom.Configuration(context.Configuration)
        // let serilog access our DI container
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

//DI Container
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddInfrastructure();

builder.Services.AddDbContext<ContactsDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnectionString")));

var app = builder.Build();

// Sample log messages
// app.Logger.LogDebug("Debug Message");
// app.Logger.LogInformation("Information Message");
// app.Logger.LogWarning("Warning Message");
// app.Logger.LogError("Error Message");
// app.Logger.LogCritical("Critical Message");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpLogging();
app.UseSerilogRequestLogging();
app.UseStaticFiles();
app.MapControllers();
app.Run();

// To make the Program class available in Integration tests
public partial class Program { }