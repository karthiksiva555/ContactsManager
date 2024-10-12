using ContactsManager.Core.Entities;
using ContactsManager.Core.Interfaces;

namespace ContactsManager.Application.Services;

/// <inheritdoc />
public class CountryService : ICountryService
{
    private readonly IList<Country> _countries = new List<Country>();

    /// <inheritdoc />
    public Country AddCountry(Country country)
    {
        if (country is null)
        {
            throw new ArgumentNullException(nameof(country));
        }

        if (string.IsNullOrEmpty(country.CountryName))
        {
            throw new ArgumentException("Country name is required", nameof(country.CountryName));
        }

        if (_countries.Any(c => c.CountryName == country.CountryName))
        {
            throw new ArgumentException("Country name must be unique", nameof(country.CountryName));
        }
        
        // This has to happen on the database side automatically
        // we are doing this here only because we are using a dummy database
        country.CountryId = Guid.NewGuid();
        _countries.Add(country);

        return country;
    }

    /// <inheritdoc />
    public IList<Country> GetAllCountries()
    {
        return _countries;
    }

    public Country? GetCountryById(Guid countryId)
    {
        if(countryId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(countryId));
        }

        return _countries.FirstOrDefault(c => c.CountryId == countryId);
    }
}