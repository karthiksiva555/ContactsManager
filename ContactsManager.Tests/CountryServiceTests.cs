using ContactsManager.Application.Services;
using ContactsManager.Core.Entities;
using ContactsManager.Core.Interfaces;

namespace ContactsManager.Tests;

public class CountryServiceTests
{
    private readonly ICountryService _countryService = new CountryService();

    #region AddCountry

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
    #endregion

    #region GetAllCountries

    [Fact]
    public void GetAllCountries_BeforeAddingACountry_ListIsEmpty()
    {
        var countries = _countryService.GetAllCountries();

        Assert.Empty(countries);
    }

    [Fact]
    public void GetAllCountries_AfterAddingCountries_ReturnsAddedCountries()
    {
        var india = new Country { CountryName = "India" };
        var canada = new Country { CountryName = "Canada" };
        _countryService.AddCountry(india);
        _countryService.AddCountry(canada);

        var countries = _countryService.GetAllCountries();

        Assert.Equal(2, countries.Count);
        Assert.Contains(india, countries);
        Assert.Contains(canada, countries);
    }

    #endregion

    #region GetCountryById

    [Fact]
    public void GetCountryById_ListIsEmpty_ReturnsNull()
    {
        var country = _countryService.GetCountryById(Guid.NewGuid());

        Assert.Null(country);
    }

    [Fact]
    public void GetCountryById_InputGuidIsNull_ThrowsArgumentNullException()
    {
        _countryService.AddCountry(new Country{ CountryName = "India" });

        Assert.Throws<ArgumentNullException>(() => _countryService.GetCountryById(Guid.Empty));
    }

    [Fact]
    public void GetCountryById_NonMatchingCountryId_ReturnsNull()
    {
        _countryService.AddCountry(new Country() { CountryName = "India" });

        var country = _countryService.GetCountryById(Guid.NewGuid());

        Assert.Null(country);
    }

    [Fact]
    public void GetCountryById_MatchingCountryIdExists_ReturnsCountry()
    {
        var countryAdded = _countryService.AddCountry(new Country() {CountryName = "India"});

        var country = _countryService.GetCountryById(countryAdded.CountryId);

        Assert.NotNull(country);
        Assert.Equal(countryAdded.CountryId, country.CountryId);
        Assert.Equal(countryAdded.CountryName, country.CountryName);
    }

    #endregion
}