using ContactsManager.Core.Entities;

namespace ContactsManager.Core.Interfaces;

public interface ICountryService
{
    /// <summary>
    /// Takes a country object as input, creates a country record in database and returns the created object
    /// The input will not contain CountryId, but the response will.
    /// </summary>
    /// <param name="country">Country to be created</param>
    /// <returns>The created country object.</returns>
    Country AddCountry(Country country);

    /// <summary>
    /// Returns all the countries found in the database
    /// </summary>
    /// <returns>A list of country objects</returns>
    IList<Country> GetAllCountries();

    /// <summary>
    /// Receives a guid as input and returns the country object that matches the guid.
    /// </summary>
    /// <param name="countryId">The ID of the country object</param>
    /// <returns>The country object with GUID as CountryId</returns>
    Country? GetCountryById(Guid countryId);
}