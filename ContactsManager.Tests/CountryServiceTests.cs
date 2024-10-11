using ContactsManager.Core.Entities;
using ContactsManager.Core.Interfaces;

namespace ContactsManager.Tests;

public class CountryServiceTests(ICountryService countryService)
{
    [Fact]
    public void AddCountry_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => countryService.AddCountry(null!));
    }

    [Fact]
    public void AddCountry_CountryNameEmpty_ThrowsArgumentException()
    {
        Country input = new () {CountryName = string.Empty};
        
        Assert.Throws<ArgumentException>(() => countryService.AddCountry(input));
    }
    
    [Fact]
    public void AddCountry_CountryNameDuplicate_ThrowsArgumentException()
    {
        Country existingCountry = new() { CountryName = "India" };
        countryService.AddCountry(existingCountry);
        Country newCountry = new() { CountryName = "India" };
        
        Assert.Throws<ArgumentException>(() => countryService.AddCountry(newCountry));
    }

    [Fact]
    public void AddCountry_ValidCountryInput_AddsCountryToDatabase()
    {
        Country input = new () {CountryName = "India"};
        
        var actual = countryService.AddCountry(input);
        
        Assert.NotEqual(Guid.Empty, actual.CountryId);
        Assert.Equal(input.CountryName, actual.CountryName);
    }
}