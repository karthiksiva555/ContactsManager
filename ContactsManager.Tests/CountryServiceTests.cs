using ContactsManager.Application.Services;
using ContactsManager.Core.Entities;
using ContactsManager.Application.DTOs;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Tests;

public class CountryServiceTests
{
    private readonly CountryService _countryService;
    private const string TestCountryName = "TestCountry";

    public CountryServiceTests()
    {
        DbContextMock<ContactsDbContext> dbContextMock = new(new DbContextOptionsBuilder<ContactsDbContext>().Options);
        dbContextMock.CreateDbSetMock(db => db.Countries, new List<Country>().AsQueryable());

        _countryService = new CountryService(dbContextMock.Object);
    }
    
    #region AddCountry

    [Fact]
    public void AddCountry_NullInput_ThrowsArgumentNullException()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _countryService.AddCountryAsync(null!));
    }

    [Fact]
    public void AddCountry_CountryNameEmpty_ThrowsArgumentException()
    {
        CountryAddRequest input = new () {CountryName = string.Empty};
        
        Assert.ThrowsAsync<ArgumentException>(async () => await _countryService.AddCountryAsync(input));
    }
    
    [Fact]
    public async Task AddCountry_CountryNameDuplicate_ThrowsArgumentException()
    {
        CountryAddRequest existingCountry = new() { CountryName = TestCountryName };
        await _countryService.AddCountryAsync(existingCountry);
        CountryAddRequest newCountry = new() { CountryName = TestCountryName };
        
        await Assert.ThrowsAsync<ArgumentException>(async () => await _countryService.AddCountryAsync(newCountry));
    }

    [Fact]
    public async Task AddCountryAsync_ValidCountryInput_AddsCountryToDatabase()
    {
        CountryAddRequest input = new () {CountryName = TestCountryName};
        
        var actual = await _countryService.AddCountryAsync(input);
        
        Assert.NotNull(actual);
        Assert.NotEqual(Guid.Empty, actual.CountryId);
        Assert.Equal(input.CountryName, actual.CountryName);
    }
    #endregion

    #region GetAllCountries

    [Fact]
    public async Task GetAllCountriesAsync_BeforeAddingACountry_ListIsEmpty()
    {
        var countries = await _countryService.GetAllCountriesAsync();

        Assert.Empty(countries);
    }

    [Fact]
    public async Task GetAllCountries_AfterAddingCountries_ReturnsAddedCountries()
    {
        await _countryService.AddCountryAsync(new CountryAddRequest() { CountryName = TestCountryName });
        await _countryService.AddCountryAsync(new CountryAddRequest() { CountryName = "New Test Country" });

        var countries = await _countryService.GetAllCountriesAsync();

        Assert.Equal(2, countries.Count);
        var countryNames = countries.Select(c => c.CountryName).ToList();
        Assert.Contains(TestCountryName, countryNames);
        Assert.Contains("New Test Country", countryNames);
    }

    #endregion

    #region GetCountryById

    [Fact]
    public async Task GetCountryById_ListIsEmpty_ReturnsNull()
    {
        var country = await _countryService.GetCountryByIdAsync(Guid.NewGuid());

        Assert.Null(country);
    }

    [Fact]
    public async Task GetCountryById_InputGuidIsNull_ThrowsArgumentNullException()
    {
        await _countryService.AddCountryAsync(new CountryAddRequest{ CountryName = TestCountryName });

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _countryService.GetCountryByIdAsync(Guid.Empty));
    }

    [Fact]
    public async Task GetCountryById_NonMatchingCountryId_ReturnsNull()
    {
        await _countryService.AddCountryAsync(new CountryAddRequest() { CountryName = TestCountryName });

        var country = await _countryService.GetCountryByIdAsync(Guid.NewGuid());

        Assert.Null(country);
    }

    [Fact]
    public async Task GetCountryById_MatchingCountryIdExists_ReturnsCountry()
    {
        var countryAdded = await _countryService.AddCountryAsync(new CountryAddRequest() {CountryName = TestCountryName});

        var countryResponse = await _countryService.GetCountryByIdAsync(countryAdded.CountryId);

        Assert.NotNull(countryResponse);
        Assert.Equal(countryAdded.CountryId, countryResponse.CountryId);
        Assert.Equal(countryAdded.CountryName, countryResponse.CountryName);
    }

    #endregion
}