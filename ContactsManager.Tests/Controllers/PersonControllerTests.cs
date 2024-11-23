using AutoFixture;
using ContactsManager.Application.DTOs;
using ContactsManager.Application.Interfaces;
using ContactsManager.Core.Enums;
using ContactsManager.Web.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ContactsManager.Tests.Controllers;

public class PersonControllerTests
{
    private readonly Mock<IPersonService> _personServiceMock;
    private readonly Mock<ICountryService> _countryServiceMock;
    private readonly PersonController _personController;
    private readonly IFixture _fixture;
    
    public PersonControllerTests()
    {
        _countryServiceMock = new Mock<ICountryService>();
        _personServiceMock = new Mock<IPersonService>();
        _fixture = new Fixture();
        _personController = new PersonController(_personServiceMock.Object, _countryServiceMock.Object);
    }

    [Fact]
    public async Task IndexAsync_ValidSearchByAndSortBy_ReturnsFilteredAndSortedPersonList()
    {
        var personList = _fixture.CreateMany<PersonResponse>().ToList();
        _personServiceMock.Setup(p => p.GetFilteredPersonsAsync(It.IsAny<string>(), It.IsAny<string?>()))
            .ReturnsAsync(personList);
        _personServiceMock.Setup(p => p.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrder>())).Returns(personList);
        
        var result = await _personController.IndexAsync(_fixture.Create<string?>(), _fixture.Create<string?>(), _fixture.Create<string>(), _fixture.Create<SortOrder>());
        
        // result should be of type ViewResult and the Model should be a PersonResponse collection
        result.Should()
            .BeOfType<ViewResult>()
            .Subject.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
    }

    [Fact]
    public async Task CreateAsync_InputHasModelErrors_ReturnsCreatedResult()
    {
        var countries = _fixture.CreateMany<CountryResponse>().ToList();
        _countryServiceMock.Setup(c => c.GetAllCountriesAsync()).ReturnsAsync(countries);
        var personToAdd = _fixture.Create<PersonAddRequest>();
        _personController.ModelState.AddModelError("PersonName", "Person name cannot be empty");
        
        var result = await _personController.CreateAsync(personToAdd);
        
        result.Should()
            .BeOfType<ViewResult>()
            .Subject.ViewData.Model.Should()
                .BeAssignableTo<PersonAddRequest>()
                .And.Be(personToAdd);
    }
    
    [Fact]
    public async Task CreateAsync_InputHasNoModelErrors_ReturnsRedirectToActionResult()
    {
        var personToAdd = _fixture.Create<PersonAddRequest>();
        _personServiceMock.Setup(p => p.AddPersonAsync(It.IsAny<PersonAddRequest>()));
        
        var result = await _personController.CreateAsync(personToAdd);
        
        result.Should()
            .BeOfType<RedirectToActionResult>()
            .Subject.ActionName.Should().Be("Index");
    }
}