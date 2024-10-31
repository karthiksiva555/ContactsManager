using ContactsManager.Core.Entities;
using ContactsManager.Application.Interfaces;
using ContactsManager.Application.DTOs;

namespace ContactsManager.Application.Services;

/// <inheritdoc />
public class CountryService : ICountryService
{
    private readonly ContactsDbContext _database;

    public CountryService(ContactsDbContext database)
    {
        _database = database;
    }

    /// <inheritdoc />
    public CountryResponse AddCountry(CountryAddRequest countryToAdd)
    {
        ArgumentNullException.ThrowIfNull(countryToAdd);

        if (string.IsNullOrEmpty(countryToAdd.CountryName))
        {
            throw new ArgumentException("Country name is required", nameof(countryToAdd.CountryName));
        }

        if (_database.Countries.Any(c => c.CountryName == countryToAdd.CountryName))
        {
            throw new ArgumentException("Country name must be unique", nameof(countryToAdd.CountryName));
        }
        
        var country = countryToAdd.ToCountry();
        _database.Countries.Add(country);
        _database.SaveChanges();
        return country.ToCountryResponse();
    }

    /// <inheritdoc />
    IList<CountryResponse> ICountryService.GetAllCountries()
    {
        List<CountryResponse> countries = [];
        var countriesFromDb = _database.Countries.ToList();
        countries.AddRange(from country in countriesFromDb
                           select country.ToCountryResponse());
        return countries;
    }

    /// <inheritdoc />
    public CountryResponse? GetCountryById(Guid countryId)
    {
        if(countryId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(countryId));
        }

        var country = _database.Countries.FirstOrDefault(c => c.CountryId == countryId);
        return country?.ToCountryResponse();
    }
}