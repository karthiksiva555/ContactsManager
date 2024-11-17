using ContactsManager.Core.Entities;

namespace ContactsManager.Core.Interfaces;

public interface ICountryRepository
{ 
    Task<Country> AddCountryAsync(Country countryToAdd);

    Task<IList<Country>> GetAllCountriesAsync();

    Task<Country?> GetCountryByIdAsync(Guid countryId);
    
    Task<bool> CountryExistsAsync(string countryName);
}