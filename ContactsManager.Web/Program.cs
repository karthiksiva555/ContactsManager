using ContactsManager.Application.Interfaces;
using ContactsManager.Application.Services;
using ContactsManager.Core.Entities;
using ContactsManager.Infrastructure;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Logging providers
// builder.Logging.ClearProviders();
// builder.Logging.AddConsole();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpLogging(options =>
{
    // Log all fields
    // options.LoggingFields = HttpLoggingFields.All;
    // Log only request properties and response headers
    options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponseBody;
});

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        // let Serilog access our config from IConfiguration
        .ReadFrom.Configuration(context.Configuration)
        // let serilog access our DI container
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();
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