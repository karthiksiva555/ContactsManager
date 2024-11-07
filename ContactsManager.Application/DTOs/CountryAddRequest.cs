using ContactsManager.Core.Entities;

namespace ContactsManager.Application.DTOs;

public class CountryAddRequest
{
    public string CountryName { get; set; } = string.Empty;
}

public static class CountryAddRequestExtensions
{
    public static Country ToCountry(this CountryAddRequest countryAddRequest)
    {
        return new Country { CountryId = Guid.NewGuid(), CountryName = countryAddRequest.CountryName };
    }
}