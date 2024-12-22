using ContactsManager.Application.DTOs;

namespace ContactsManager.Application.ServiceInterfaces;

public interface ICountryService
{
    /// <summary>
    /// Adds a country object to the list of countries
    /// </summary>
    /// <param name="countryToAdd">The country to be added supplied in the form of a DTO</param>
    /// <returns>The country that was just added, returned as a DTO</returns>
    Task<CountryResponse> AddCountryAsync(CountryAddRequest countryToAdd);

    /// <summary>
    /// Returns all the countries that are previously added to the database.
    /// </summary>
    /// <returns>The list of countries</returns>
    Task<IList<CountryResponse>> GetAllCountriesAsync();

    /// <summary>
    /// Accepts a country id as input and returns the matching country object found in the database.
    /// </summary>
    /// <param name="countryId">The Id of the country to be searched with.</param>
    /// <returns>The matching country object, null if nothing matches.</returns>
    Task<CountryResponse?>? GetCountryByIdAsync(Guid countryId);
}