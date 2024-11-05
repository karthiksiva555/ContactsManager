using ContactsManager.Core.Entities;
using ContactsManager.Application.Interfaces;
using ContactsManager.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Application.Services;

/// <inheritdoc />
public class CountryService(ContactsDbContext database) : ICountryService
{
    /// <inheritdoc />
    public async Task<CountryResponse?> AddCountryAsync(CountryAddRequest countryToAdd)
    {
        ArgumentNullException.ThrowIfNull(countryToAdd);

        if (string.IsNullOrEmpty(countryToAdd.CountryName))
        {
            throw new ArgumentException("Country name is required", nameof(countryToAdd.CountryName));
        }

        if (database.Countries.Any(c => c.CountryName == countryToAdd.CountryName))
        {
            throw new ArgumentException("Country name must be unique", nameof(countryToAdd.CountryName));
        }
        
        var country = countryToAdd.ToCountry();
        database.Countries.Add(country);
        await database.SaveChangesAsync();
        return country.ToCountryResponse();
    }

    /// <inheritdoc />
    public async Task<IList<CountryResponse>> GetAllCountriesAsync()
    {
        List<CountryResponse> countries = [];
        var countriesFromDb = await database.Countries.ToListAsync();
        countries.AddRange(from country in countriesFromDb
                           select country.ToCountryResponse());
        return countries;
    }

    /// <inheritdoc />
    public async Task<CountryResponse?> GetCountryByIdAsync(Guid countryId)
    {
        if(countryId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(countryId));
        }

        var country = await database.Countries.FirstOrDefaultAsync(c => c.CountryId == countryId);
        return country?.ToCountryResponse();
    }
}