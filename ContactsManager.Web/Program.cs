using ContactsManager.Web.Extensions;
using ContactsManager.Web.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Logging providers
// builder.Logging.ClearProviders();
// builder.Logging.AddConsole();

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        // let Serilog access our config from IConfiguration
        .ReadFrom.Configuration(context.Configuration)
        // let serilog access our DI container
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

// Delegate all the services to an extension method
builder.Services.ConfigureServices(builder.Configuration);

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
else
{
    app.UseExceptionHandler("/error");
    app.UseCustomExceptionHandling();
}

// To redirect the user to a status code specific error page
// app.UseStatusCodePagesWithRedirects("/error/{0}");

app.UseHttpLogging();
app.UseSerilogRequestLogging();
app.UseStaticFiles();
app.MapControllers();
app.Run();

// To make the Program class available in Integration tests
public partial class Program { }