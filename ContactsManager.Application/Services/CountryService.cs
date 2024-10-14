using ContactsManager.Core.Entities;
using ContactsManager.Application.Interfaces;
using ContactsManager.Application.DTOs;

namespace ContactsManager.Application.Services;

/// <inheritdoc />
public class CountryService : ICountryService
{
    private readonly IList<Country> _countries = [];

    /// <inheritdoc />
    public CountryResponse AddCountry(CountryAddRequest countryToAdd)
    {
        
        if (countryToAdd is null)
        {
            throw new ArgumentNullException(nameof(countryToAdd));
        }

        if (string.IsNullOrEmpty(countryToAdd.CountryName))
        {
            throw new ArgumentException("Country name is required", nameof(countryToAdd.CountryName));
        }

        if (_countries.Any(c => c.CountryName == countryToAdd.CountryName))
        {
            throw new ArgumentException("Country name must be unique", nameof(countryToAdd.CountryName));
        }
        
        Country country = countryToAdd.ToCountry();
        // This has to happen on the database side automatically
        // we are doing this here only because we are using a dummy database
        country.CountryId = Guid.NewGuid();
        _countries.Add(country);

        return country.ToCountryResponse();
    }

    /// <inheritdoc />
    IList<CountryResponse> ICountryService.GetAllCountries()
    {
        List<CountryResponse> countries = [];
        countries.AddRange(from country in _countries
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

        var country = _countries.FirstOrDefault(c => c.CountryId == countryId);
        return country?.ToCountryResponse();
    }
}