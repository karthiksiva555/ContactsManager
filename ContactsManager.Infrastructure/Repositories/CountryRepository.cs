using ContactsManager.Core.Entities;
using ContactsManager.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Infrastructure.Repositories;

public class CountryRepository(ContactsDbContext contactsDbContext) : ICountryRepository
{
    public async Task<Country> AddCountryAsync(Country countryToAdd)
    {
        await contactsDbContext.Countries.AddAsync(countryToAdd);
        await contactsDbContext.SaveChangesAsync();
        
        return countryToAdd;
    }

    public async Task<IList<Country>> GetAllCountriesAsync()
    {
        return await contactsDbContext.Countries.ToListAsync();
    }

    public async Task<Country?> GetCountryByIdAsync(Guid countryId)
    {
        return await contactsDbContext.Countries.FirstOrDefaultAsync(c => c.CountryId == countryId);
    }

    public async Task<bool> CountryExistsAsync(string countryName)
    {
        return await contactsDbContext.Countries.AnyAsync(c => countryName.Equals(c.CountryName, StringComparison.OrdinalIgnoreCase));
    }
}