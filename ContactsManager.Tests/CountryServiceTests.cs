using ContactsManager.Application.Services;
using ContactsManager.Core.Entities;
using ContactsManager.Core.Interfaces;

namespace ContactsManager.Tests;

public class CountryServiceTests
{
    private readonly ICountryService _countryService = new CountryService();

    [Fact]
    public void AddCountry_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _countryService.AddCountry(null!));
    }

    [Fact]
    public void AddCountry_CountryNameEmpty_ThrowsArgumentException()
    {
        Country input = new () {CountryName = string.Empty};
        
        Assert.Throws<ArgumentException>(() => _countryService.AddCountry(input));
    }
    
    [Fact]
    public void AddCountry_CountryNameDuplicate_ThrowsArgumentException()
    {
        Country existingCountry = new() { CountryName = "India" };
        _countryService.AddCountry(existingCountry);
        Country newCountry = new() { CountryName = "India" };
        
        Assert.Throws<ArgumentException>(() => _countryService.AddCountry(newCountry));
    }

    [Fact]
    public void AddCountry_ValidCountryInput_AddsCountryToDatabase()
    {
        Country input = new () {CountryName = "India"};
        
        var actual = _countryService.AddCountry(input);
        
        Assert.NotEqual(Guid.Empty, actual.CountryId);
        Assert.Equal(input.CountryName, actual.CountryName);
    }
}