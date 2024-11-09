using ContactsManager.Core.Entities;

namespace ContactsManager.Application.DTOs;

public class CountryResponse
{
    public Guid CountryId { get; set; }
    
    public string? CountryName { get; set; } = string.Empty;
}

public static class CountryResponseExtensions 
{
    public static CountryResponse ToCountryResponse(this Country country)
    {
        return new CountryResponse() { CountryId = country.CountryId, CountryName = country.CountryName };
    }
}