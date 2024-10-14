using ContactsManager.Application.DTOs;
using ContactsManager.Application.Interfaces;
using ContactsManager.Application.Services;
using ContactsManager.Core.Entities;
using ContactsManager.Core.Enums;

namespace ContactsManager.Tests;

public class PersonServiceTests
{
    private readonly ICountryService _countryService;
    private readonly PersonService _personService;

    public PersonServiceTests()
    {
        _countryService = new CountryService();
        _personService = new PersonService(_countryService);
    }

    #region AddPerson

    [Fact]
    public void AddPerson_InputIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _personService.AddPerson(null!));
    }

    [Fact]
    public void AddPerson_PersonNameIsNull_ThrowsArgumentException()
    {
        PersonAddRequest personToAdd = new() { PersonName = null!};

        Assert.Throws<ArgumentException>(() => _personService.AddPerson(personToAdd));
    }

    [Fact]
    public void AddPerson_EmailAddressIsNotValid_ThrowsArgumentException()
    {
        PersonAddRequest personToAdd = new() { PersonName = "Test Person", EmailAddress = "test.com"};

        var exception = Assert.Throws<ArgumentException>(() => _personService.AddPerson(personToAdd));
        Assert.Equal("Email Address must be in valid format.", exception.Message);
    }

    [Fact]
    public void AddPerson_PersonInputIsValid_ReturnsAddedPerson()
    {
        CountryAddRequest countryToAdd = new() { CountryName = "India" };
        CountryResponse countryResponse = _countryService.AddCountry(countryToAdd);
        PersonAddRequest personToAdd = new() { PersonName = "Siva", DateOfBirth = new DateTime(1989, 05, 24), Gender = Gender.Male, EmailAddress = "test@tes.com", CountryId = countryResponse.CountryId};

        PersonResponse addedPerson = _personService.AddPerson(personToAdd);

        Assert.NotEqual(Guid.Empty, addedPerson.PersonId);
        Assert.Equal(personToAdd.PersonName, addedPerson.PersonName);
        var expectedAge = personToAdd.DateOfBirth?.Year - DateTime.Now.Year;
        Assert.Equal(expectedAge, addedPerson.Age);
        Assert.Equal(personToAdd.EmailAddress, addedPerson.EmailAddress);
        Assert.Equal(countryResponse.CountryName, addedPerson.Country?.CountryName);
    }

    #endregion

    #region GetAllPersons

    [Fact]
    public void GetAllPersons_BeforeInitialization_ContainsEmptyList()
    {
        var persons = _personService.GetAllPersons();

        Assert.Empty(persons);
    }

    [Fact]
    public void GetAllPersons_AfterInitialization_ReturnsAddedPersons()
    {
        _personService.AddPerson(new PersonAddRequest(){ PersonName = "Ram"});
        _personService.AddPerson(new PersonAddRequest() { PersonName = "Robert"});

        var persons = _personService.GetAllPersons();

        Assert.Equal(2, persons.Count);
        var personNames = from person in persons select person.PersonName;
        Assert.Contains("Ram", personNames);
        Assert.Contains("Robert", personNames);
    }

    #endregion

    #region GetPersonById

    [Fact]
    public void GetPersonById_NullPersonId_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _personService.GetPersonById(Guid.Empty));
    }

    [Fact]
    public void GetPersonById_PersonListIsEmpty_ReturnsNull()
    {
        PersonResponse? person = _personService.GetPersonById(Guid.NewGuid());

        Assert.Null(person);
    }

    [Fact]
    public void GetPersonById_NoMatchingPersonFound_ReturnsNull()
    {
        _personService.AddPerson(new PersonAddRequest(){ PersonName = "Rahim"});

        var person = _personService.GetPersonById(Guid.NewGuid());

        Assert.Null(person);
    }

    [Fact]
    public void GetPersonById_MatchingPersonFound_ReturnsMatchingPerson()
    {
        var personAdded = _personService.AddPerson(new PersonAddRequest(){ PersonName = "Rahim"});

        var person = _personService.GetPersonById(personAdded.PersonId);

        Assert.NotNull(person);
        Assert.Equal(personAdded.PersonName, person.PersonName);
        Assert.Equal(personAdded.PersonId, person.PersonId);
    }

    #endregion
}