using ContactsManager.Application.Services;
using ContactsManager.Core.Entities;
using ContactsManager.Application.Interfaces;
using ContactsManager.Application.DTOs;

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
        CountryAddRequest input = new () {CountryName = string.Empty};
        
        Assert.Throws<ArgumentException>(() => _countryService.AddCountry(input));
    }
    
    [Fact]
    public void AddCountry_CountryNameDuplicate_ThrowsArgumentException()
    {
        CountryAddRequest existingCountry = new() { CountryName = "India" };
        _countryService.AddCountry(existingCountry);
        CountryAddRequest newCountry = new() { CountryName = "India" };
        
        Assert.Throws<ArgumentException>(() => _countryService.AddCountry(newCountry));
    }

    [Fact]
    public void AddCountry_ValidCountryInput_AddsCountryToDatabase()
    {
        CountryAddRequest input = new () {CountryName = "India"};
        
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
        _countryService.AddCountry(new CountryAddRequest() { CountryName = "India" });
        _countryService.AddCountry(new CountryAddRequest() { CountryName = "Canada" });

        var countries = _countryService.GetAllCountries();

        Assert.Equal(2, countries.Count);
        var countryNames = from country in countries select country.CountryName;
        Assert.Contains("India", countryNames);
        Assert.Contains("Canada", countryNames);
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
        _countryService.AddCountry(new CountryAddRequest{ CountryName = "India" });

        Assert.Throws<ArgumentNullException>(() => _countryService.GetCountryById(Guid.Empty));
    }

    [Fact]
    public void GetCountryById_NonMatchingCountryId_ReturnsNull()
    {
        _countryService.AddCountry(new CountryAddRequest() { CountryName = "India" });

        var country = _countryService.GetCountryById(Guid.NewGuid());

        Assert.Null(country);
    }

    [Fact]
    public void GetCountryById_MatchingCountryIdExists_ReturnsCountry()
    {
        var countryAdded = _countryService.AddCountry(new CountryAddRequest() {CountryName = "India"});

        var countryResponse = _countryService.GetCountryById(countryAdded.CountryId);

        Assert.NotNull(countryResponse);
        Assert.Equal(countryAdded.CountryId, countryResponse.CountryId);
        Assert.Equal(countryAdded.CountryName, countryResponse.CountryName);
    }

    #endregion
}