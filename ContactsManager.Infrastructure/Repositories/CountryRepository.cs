using ContactsManager.Core.Entities;
using ContactsManager.Core.Interfaces;

namespace ContactsManager.Infrastructure.Repositories;

public class CountryRepository(ContactsDbContext contactsDbContext) : ICountryRepository
{
    public Task<Country> AddCountryAsync(Country countryToAdd)
    {
        throw new NotImplementedException();
    }

    public Task<IList<Country>> GetAllCountriesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Country?> GetCountryByIdAsync(Guid countryId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CountryExistsAsync(string countryName)
    {
        throw new NotImplementedException();
    }
}