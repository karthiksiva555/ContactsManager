using ContactsManager.Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContactsManager.IntegrationTests;

// The Program class here is taking Program.cs from ContactsManager.Web as base
// Any changes made in this class takes the precedence over the base
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Configures everything needed for the sample application to be available for the integration tests
    /// Overrides the default config created in Program.cs
    /// </summary>
    /// <param name="builder">The application builder</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        
        // Override the environment
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            // Remove the real database
            var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<ContactsDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            // Add the dummy database for integration tests
            services.AddDbContext<ContactsDbContext>(options =>
            {
                options.UseInMemoryDatabase("ContactsDbIntegrationTest");
            });
        });
    }
}