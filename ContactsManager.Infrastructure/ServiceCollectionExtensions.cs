using ContactsManager.Application.RepositoryInterfaces;
using ContactsManager.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ContactsManager.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        
        return services;
    }
}