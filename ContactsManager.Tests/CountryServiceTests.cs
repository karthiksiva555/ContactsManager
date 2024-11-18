using AutoFixture;
using ContactsManager.Application.Services;
using ContactsManager.Core.Entities;
using ContactsManager.Application.DTOs;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Tests;

public class CountryServiceTests
{
    private readonly CountryService _countryService;
    private readonly Fixture _fixture;

    public CountryServiceTests()
    {
        _fixture = new Fixture();
        DbContextMock<ContactsDbContext> dbContextMock = new(new DbContextOptionsBuilder<ContactsDbContext>().Options);
        dbContextMock.CreateDbSetMock(db => db.Countries, new List<Country>().AsQueryable());

        _countryService = new CountryService(null);
    }
    
    #region AddCountry

    [Fact]
    public async Task AddCountryAsync_NullInput_ThrowsArgumentNullException()
    {
        Func<Task> action = async () => await _countryService.AddCountryAsync(null!);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddCountryAsync_CountryNameEmpty_ThrowsArgumentException()
    {
        var input = _fixture.Build<CountryAddRequest>()
            .With(c => c.CountryName, string.Empty).Create();
        
        Func<Task> action = async () => await _countryService.AddCountryAsync(input);
        await action.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task AddCountryAsync_CountryNameDuplicate_ThrowsArgumentException()
    {
        var existingCountry = _fixture.Create<CountryAddRequest>();
        await _countryService.AddCountryAsync(existingCountry);

        var newCountry = _fixture.Build<CountryAddRequest>()
            .With(c => c.CountryName, existingCountry.CountryName).Create();
        
        Func<Task> action = async () => await _countryService.AddCountryAsync(newCountry);
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddCountryAsync_ValidCountryInput_AddsCountryToDatabase()
    {
        var input = _fixture.Create<CountryAddRequest>();
        
        var actual = await _countryService.AddCountryAsync(input);
        
        actual.Should().NotBeNull();
        actual.CountryId.Should().NotBeEmpty();
        actual.CountryName.Should().Be(input.CountryName);
    }
    #endregion

    #region GetAllCountries

    [Fact]
    public async Task GetAllCountriesAsync_BeforeAddingACountry_ListIsEmpty()
    {
        var countries = await _countryService.GetAllCountriesAsync();

        countries.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCountriesAsync_AfterAddingCountries_ReturnsAddedCountries()
    {
        var country1 = _fixture.Create<CountryAddRequest>();
        var country2 = _fixture.Create<CountryAddRequest>();
        await _countryService.AddCountryAsync(country1);
        await _countryService.AddCountryAsync(country2);

        var countries = await _countryService.GetAllCountriesAsync();

        countries.Should().HaveCount(2);
        countries.Select(c => c.CountryName).Should().Contain(new[] {country1.CountryName, country2.CountryName});
    }

    #endregion

    #region GetCountryById

    [Fact]
    public async Task GetCountryByIdAsync_ListIsEmpty_ReturnsNull()
    {
        var country = await _countryService.GetCountryByIdAsync(Guid.NewGuid());

        country.Should().BeNull();
    }

    [Fact]
    public async Task GetCountryById_InputGuidIsNull_ThrowsArgumentNullException()
    {
        await _countryService.AddCountryAsync(_fixture.Create<CountryAddRequest>());

        Func<Task> action = async () => await _countryService.GetCountryByIdAsync(Guid.Empty);
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetCountryByIdAsync_NonMatchingCountryId_ReturnsNull()
    {
        await _countryService.AddCountryAsync(_fixture.Create<CountryAddRequest>());

        var country = await _countryService.GetCountryByIdAsync(Guid.NewGuid());

        country.Should().BeNull();
    }

    [Fact]
    public async Task GetCountryByIdAsync_MatchingCountryIdExists_ReturnsCountry()
    {
        var countryAdded = await _countryService.AddCountryAsync(_fixture.Create<CountryAddRequest>());

        var countryResponse = await _countryService.GetCountryByIdAsync(countryAdded.CountryId);

        countryResponse.Should().NotBeNull();
        countryResponse?.CountryId.Should().Be(countryAdded.CountryId);
        countryResponse?.CountryName.Should().Be(countryAdded.CountryName);
    }

    #endregion
}