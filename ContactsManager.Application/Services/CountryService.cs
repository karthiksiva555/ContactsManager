using ContactsManager.Application.ServiceInterfaces;
using ContactsManager.Application.DTOs;
using ContactsManager.Application.RepositoryInterfaces;

namespace ContactsManager.Application.Services;

/// <inheritdoc />
public class CountryService(ICountryRepository countryRepository) : ICountryService
{
    /// <inheritdoc />
    public async Task<CountryResponse> AddCountryAsync(CountryAddRequest countryToAdd)
    {
        ArgumentNullException.ThrowIfNull(countryToAdd);

        if (string.IsNullOrEmpty(countryToAdd.CountryName))
        {
            throw new ArgumentException("Country name is required", nameof(countryToAdd.CountryName));
        }

        if (await countryRepository.CountryExistsAsync(countryToAdd.CountryName))
        {
            throw new ArgumentException("Country name must be unique", nameof(countryToAdd.CountryName));
        }
        
        var country = await countryRepository.AddCountryAsync(countryToAdd.ToCountry());
        return country.ToCountryResponse();
    }

    /// <inheritdoc />
    public async Task<IList<CountryResponse>> GetAllCountriesAsync()
    {
        List<CountryResponse> countries = [];
        var countriesFromDb = await countryRepository.GetAllCountriesAsync();
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

        var country = await countryRepository.GetCountryByIdAsync(countryId);
        return country?.ToCountryResponse();
    }
}