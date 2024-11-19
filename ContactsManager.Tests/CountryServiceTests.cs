using AutoFixture;
using ContactsManager.Application.Services;
using ContactsManager.Core.Entities;
using ContactsManager.Application.DTOs;
using ContactsManager.Core.Interfaces;
using FluentAssertions;
using Moq;

namespace ContactsManager.Tests;

public class CountryServiceTests
{
    private readonly CountryService _countryService;
    private readonly Fixture _fixture;
    private readonly Mock<ICountryRepository> _countryRepositoryMock;

    public CountryServiceTests()
    {
        _fixture = new Fixture();
        _countryRepositoryMock = new Mock<ICountryRepository>(); 
        _countryService = new CountryService(_countryRepositoryMock.Object);
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
        _countryRepositoryMock.Setup(c => c.CountryExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        var newCountry = _fixture.Create<CountryAddRequest>();
        
        Func<Task> action = async () => await _countryService.AddCountryAsync(newCountry);
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddCountryAsync_ValidCountryInput_AddsCountryToDatabase()
    {
        var input = _fixture.Create<CountryAddRequest>();
        _countryRepositoryMock.Setup(c => c.AddCountryAsync(It.IsAny<Country>())).ReturnsAsync(input.ToCountry());
        
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
        _countryRepositoryMock.Setup(c => c.GetAllCountriesAsync()).ReturnsAsync(new List<Country>());
        
        var countries = await _countryService.GetAllCountriesAsync();

        countries.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCountriesAsync_AfterAddingCountries_ReturnsAddedCountries()
    {
        var country1 = _fixture.Create<Country>();
        var country2 = _fixture.Create<Country>();
        var countriesInDb = new[] { country1, country2 };
        _countryRepositoryMock.Setup(c => c.GetAllCountriesAsync()).ReturnsAsync(countriesInDb);
        
        var countries = await _countryService.GetAllCountriesAsync();

        countries.Should().HaveCount(2);
        countries.Select(c => c.CountryName).Should().Contain(new[] {country1.CountryName, country2.CountryName});
    }

    #endregion

    #region GetCountryById

    [Fact]
    public async Task GetCountryByIdAsync_ListIsEmpty_ReturnsNull()
    {
        _countryRepositoryMock.Setup(c => c.GetCountryByIdAsync(It.IsAny<Guid>())).ReturnsAsync(null as Country);
        var country = await _countryService.GetCountryByIdAsync(Guid.NewGuid());

        country.Should().BeNull();
    }

    [Fact]
    public async Task GetCountryById_InputGuidIsNull_ThrowsArgumentNullException()
    {
        Func<Task> action = async () => await _countryService.GetCountryByIdAsync(Guid.Empty);
        
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetCountryByIdAsync_NonMatchingCountryId_ReturnsNull()
    {
        _countryRepositoryMock.Setup(c => c. GetCountryByIdAsync(It.IsAny<Guid>())).ReturnsAsync(null as Country);
        
        var country = await _countryService.GetCountryByIdAsync(Guid.NewGuid());

        country.Should().BeNull();
    }

    [Fact]
    public async Task GetCountryByIdAsync_MatchingCountryIdExists_ReturnsCountry()
    {
        var country = _fixture.Create<Country>();
        _countryRepositoryMock.Setup(c => c.GetCountryByIdAsync(It.IsAny<Guid>())).ReturnsAsync(country);

        var countryResponse = await _countryService.GetCountryByIdAsync(country.CountryId);

        countryResponse.Should().NotBeNull();
        countryResponse?.CountryId.Should().Be(country.CountryId);
        countryResponse?.CountryName.Should().Be(country.CountryName);
    }

    #endregion
}