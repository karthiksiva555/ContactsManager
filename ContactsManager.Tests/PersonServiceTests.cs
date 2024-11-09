using AutoFixture;
using ContactsManager.Application.DTOs;
using ContactsManager.Application.Services;
using ContactsManager.Core.Entities;
using ContactsManager.Core.Enums;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace ContactsManager.Tests;

public class PersonServiceTests
{
    private readonly CountryService _countryService;
    private readonly PersonService _personService;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IFixture _fixture;

    public PersonServiceTests(ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture();
        
        DbContextMock<ContactsDbContext> dbContextMock = new(new DbContextOptionsBuilder<ContactsDbContext>().Options);
        
        dbContextMock.CreateDbSetMock(db => db.Countries, new List<Country>().AsQueryable());
        dbContextMock.CreateDbSetMock(db => db.Persons, new List<Person>().AsQueryable());
        
        _countryService = new CountryService(dbContextMock.Object);
        _personService = new PersonService(_countryService, dbContextMock.Object);
        _testOutputHelper = testOutputHelper;
    }

    #region Private Methods

    private static string[] GetPersonNames(IList<PersonResponse> persons)
    {
        var personNames = from person in persons select person.PersonName;
        return personNames as string[] ?? personNames.ToArray();
    }
    
    #endregion Private Methods
    
    #region AddPerson

    [Fact]
    public async Task AddPersonAsync_InputIsNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personService.AddPersonAsync(null!));
    }

    [Fact]
    public async Task AddPersonAsync_PersonNameIsNull_ThrowsArgumentException()
    {
        // PersonAddRequest personToAdd = new() { PersonName = null!};
        var personToAdd = _fixture.Build<PersonAddRequest>()
            .With(p => p.PersonName, null! as string).Create();

        await Assert.ThrowsAsync<ArgumentException>(async () => await _personService.AddPersonAsync(personToAdd));
    }

    [Fact]
    public async Task AddPersonAsync_EmailAddressIsNotValid_ThrowsArgumentException()
    {
        // PersonAddRequest personToAdd = new() { PersonName = "Test Person", EmailAddress = "test.com"};
        var personToAdd = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test.com").Create();

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _personService.AddPersonAsync(personToAdd));
        Assert.Equal("Email Address must be in valid format.", exception.Message);
    }

    [Fact]
    public async Task AddPersonAsync_PersonInputIsValid_ReturnsAddedPerson()
    {
        // CountryAddRequest countryToAdd = new() { CountryName = "Test Country" };
        // var countryResponse = await _countryService.AddCountryAsync(countryToAdd);
        // // PersonAddRequest personToAdd = new() { PersonName = "Siva", DateOfBirth = new DateTime(1989, 05, 24), Gender = Gender.Male, EmailAddress = "test@tes.com", CountryId = countryResponse?.CountryId };
        
        // This will generate dummy values for each property
        // var personToAdd = _fixture.Create<PersonAddRequest>();

        // This will let us override specific property values
        var personToAdd = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test@test.com").Create();
        
        _testOutputHelper.WriteLine("Calling the Add Person method");
        PersonResponse addedPerson = await _personService.AddPersonAsync(personToAdd);

        Assert.NotEqual(Guid.Empty, addedPerson.PersonId);
        Assert.Equal(personToAdd.PersonName, addedPerson.PersonName);
        var expectedAge = DateTime.Now.Year - personToAdd.DateOfBirth?.Year;
        _testOutputHelper.WriteLine($"Verifying the age property is calculated correctly");
        Assert.Equal(expectedAge, addedPerson.Age);
        Assert.Equal(personToAdd.EmailAddress, addedPerson.EmailAddress);
        // Unit Testing navigation props is not easy in EF Core
        // This will fail as the person entity won't fetch Country object even with .Include() on Person fetch
        //Assert.Equal(countryResponse?.CountryName, addedPerson.Country);
    }

    #endregion

    #region GetAllPersons

    [Fact]
    public async Task GetAllPersons_BeforeInitialization_ContainsEmptyList()
    {
        var persons = await _personService.GetAllPersonsAsync();

        Assert.Empty(persons);
    }

    [Fact]
    public async Task GetAllPersons_AfterInitialization_ReturnsAddedPersons()
    {
        var person1=_fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test1@test.com").Create();
        var person2=_fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test2@test.com").Create();
        await _personService.AddPersonAsync(person1);
        await _personService.AddPersonAsync(person2);

        var persons = await _personService.GetAllPersonsAsync();
        var personNames = GetPersonNames(persons);
        Assert.Equal(2, persons.Count);
        
        Assert.Contains(person1.PersonName, personNames);
        Assert.Contains(person2.PersonName, personNames);
    }
    
    #endregion

    #region GetPersonById

    [Fact]
    public async Task GetPersonById_NullPersonId_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _personService.GetPersonByIdAsync(Guid.Empty));
    }

    [Fact]
    public async Task GetPersonById_PersonListIsEmpty_ReturnsNull()
    {
        PersonResponse? person = await _personService.GetPersonByIdAsync(Guid.NewGuid());

        Assert.Null(person);
    }

    [Fact]
    public async Task GetPersonById_NoMatchingPersonFound_ReturnsNull()
    {
        var personToAdd = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test@test.com").Create();
        await _personService.AddPersonAsync(personToAdd);

        var person = await _personService.GetPersonByIdAsync(Guid.NewGuid());

        Assert.Null(person);
    }

    [Fact]
    public async Task GetPersonById_MatchingPersonFound_ReturnsMatchingPerson()
    {
        // var personAdded = await _personService.AddPersonAsync(new PersonAddRequest(){ PersonName = "Rahim"});
        var personToAdd = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test@test.com").Create();
        var personAdded = await _personService.AddPersonAsync(personToAdd);
        
        var person = await _personService.GetPersonByIdAsync(personAdded.PersonId);

        Assert.NotNull(person);
        Assert.Equal(personAdded.PersonName, person.PersonName);
        Assert.Equal(personAdded.PersonId, person.PersonId);
    }

    #endregion
    
    #region GetFilteredPersons

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("InvalidPropertyName")]
    public async Task GetFilteredPersons_InvalidSearchByValue_ThrowsArgumentException(string searchBy)
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _personService.GetFilteredPersonsAsync(searchBy, "any"));
    }

    [Fact]
    public async Task GetFilteredPersons_PersonListIsEmpty_ReturnsEmptyList()
    {
        // Person list is empty in the beginning
        var filteredPersons = await _personService.GetFilteredPersonsAsync("PersonName", "Si");
        
        Assert.Empty(filteredPersons);
    }

    [Fact]
    public async Task GetFilteredPersons_EmptySearchString_ReturnsAllPersons()
    {
        var person1 = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test1@test.com").Create();
        var person2 = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test2@test.com").Create();
        var person3 = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test3@test.com").Create();
        await _personService.AddPersonAsync(person1);
        await _personService.AddPersonAsync(person2);
        await _personService.AddPersonAsync(person3);
        
        var filteredPersons = await _personService.GetFilteredPersonsAsync("PersonName", "");
        
        Assert.Equal(3, filteredPersons.Count);
        var personNames = GetPersonNames(filteredPersons);
        Assert.Contains(person1.PersonName, personNames);
        Assert.Contains(person2.PersonName, personNames);
        Assert.Contains(person3.PersonName, personNames);
    }

    [Fact]
    public async Task GetFilteredPersons_SearchStringIsValid_ReturnsFilteredPersons()
    {
        var person1 = _fixture.Build<PersonAddRequest>()
            .With(p => p.PersonName, "Ram")
            .With(p => p.EmailAddress, "test1@test.com").Create();
        var person2 = _fixture.Build<PersonAddRequest>()
            .With(p => p.PersonName, "rahim")
            .With(p => p.EmailAddress, "test2@test.com").Create();
        var person3 = _fixture.Build<PersonAddRequest>()
            .With(p => p.PersonName, "Robert")
            .With(p => p.EmailAddress, "test3@test.com").Create();
        await _personService.AddPersonAsync(person1);
        await _personService.AddPersonAsync(person2);
        await _personService.AddPersonAsync(person3);
        
        var filteredPersons = await _personService.GetFilteredPersonsAsync("PersonName", "ra");
        
        Assert.Equal(2, filteredPersons.Count);
        var personNames = GetPersonNames(filteredPersons);
        Assert.Contains("rahim", personNames);
        Assert.Contains("Ram", personNames);
    }
    
    #endregion

    #region GetSortedPersons

    [Fact]
    public void GetSortedPersons_EmptyPersonsList_ReturnsEmptyList()
    {
        List<PersonResponse> persons = [];

        var sortedPersons = _personService.GetSortedPersons(persons, "PersonName", SortOrder.Asc);
        
        Assert.Empty(sortedPersons);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("InvalidPropertyName")]
    public void GetSortedPersons_InvalidSortByValue_ThrowsArgumentException(string sortBy)
    {
        // Not using fixture here as this doesn't need a person to be created in dbcontext
        List<PersonResponse> persons = [
            new() { PersonName = "Hanuman"}
        ];
        
        Assert.Throws<ArgumentException>(() => _personService.GetSortedPersons(persons, sortBy, SortOrder.Asc));
    }

    [Fact]
    public void GetSortedPersons_SortByPersonNameAscending_ReturnsSortedPersons()
    {
        List<PersonResponse> persons =
        [
            new() { PersonName = "Rama", Age = 25 },
            new() { PersonName = "Seetha", Age = 30 },
            new() { PersonName = "Lakshmana", Age = 22 }
        ];
        
        var sortedPersons = _personService.GetSortedPersons(persons, "PersonName", SortOrder.Asc);
        
        Assert.Equal(3, sortedPersons.Count);
        Assert.Equal("Lakshmana", sortedPersons[0].PersonName);
        Assert.Equal("Rama", sortedPersons[1].PersonName);
        Assert.Equal("Seetha", sortedPersons[2].PersonName);
    }
    
    [Fact]
    public void GetSortedPersons_SortByPersonNameDescending_ReturnsSortedPersons()
    {
        List<PersonResponse> persons =
        [
            new() { PersonName = "Rama", Age = 25 },
            new() { PersonName = "Seetha", Age = 30 },
            new() { PersonName = "Lakshmana", Age = 22 }
        ];
        
        var sortedPersons = _personService.GetSortedPersons(persons, "PersonName", SortOrder.Desc);
        
        Assert.Equal(3, sortedPersons.Count);
        Assert.Equal("Seetha", sortedPersons[0].PersonName);
        Assert.Equal("Rama", sortedPersons[1].PersonName);
        Assert.Equal("Lakshmana", sortedPersons[2].PersonName);
    }
    
    #endregion

    #region UpdatePerson

    [Fact]
    public async Task UpdatePerson_PersonUpdateRequestIsNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _personService.UpdatePersonAsync(null!));
    }

    [Fact]
    public async Task UpdatePerson_PersonIdIsNull_ThrowsArgumentException()
    {
        // PersonUpdateRequest personUpdateRequest = new() { PersonName = "Test" };
        var personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
            .With(p => p.EmailAddress, "test@test.com").Create();
        
        await Assert.ThrowsAsync<ArgumentException>(async () => await _personService.UpdatePersonAsync(personUpdateRequest));
    }

    [Fact]
    public async Task UpdatePerson_PersonIdDoesNotExist_ThrowsArgumentException()
    {
        var person1 = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test1@test.com").Create();
        var person2 = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test2@test.com").Create();
        await _personService.AddPersonAsync(person1);
        await _personService.AddPersonAsync(person2);
        
        // var personToUpdate = new PersonUpdateRequest() { PersonId = Guid.NewGuid(), PersonName = "Update Test"};
        var personToUpdate = _fixture.Build<PersonUpdateRequest>().With(p => p.EmailAddress, "test3@test.com").Create();

        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _personService.UpdatePersonAsync(personToUpdate));
        Assert.Equal("Invalid argument supplied (Parameter \'PersonId\')", exception.Message);
    }

    [Fact]
    public async Task UpdatePerson_ValidPersonUpdateRequest_PersonIsUpdated()
    {
        var personAddRequest1 = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test1@test.com").Create();
        var personAddRequest2 = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test2@test.com").Create();
        var person1 = await _personService.AddPersonAsync(personAddRequest1);
        await _personService.AddPersonAsync(personAddRequest2);
        
        // var personToUpdate = new PersonUpdateRequest() { PersonId = person1.PersonId, PersonName = "Updated Name"};
        var personToUpdate = _fixture.Build<PersonUpdateRequest>()
            .With(p => p.PersonId, person1.PersonId)
            .With(p => p.PersonName, "Updated Name")
            .With(p => p.EmailAddress, "test3@test.com").Create();
        
        var updatedPerson = await _personService.UpdatePersonAsync(personToUpdate);
        
        Assert.Equal(personToUpdate.PersonName, updatedPerson.PersonName);
        var person = await _personService.GetPersonByIdAsync(person1.PersonId);
        Assert.NotNull(person);
        Assert.Equal(personToUpdate.PersonName, person.PersonName);
    }
    
    #endregion

    #region DeletePerson

    [Fact]
    public async Task DeletePerson_PersonIdIsInvalid_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(async () => await _personService.DeletePersonAsync(Guid.Empty));
    }

    [Fact]
    public async Task DeletePerson_PersonIdDoesNotExistInList_ReturnsFalse()
    {
        var personToAdd = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test1@test.com").Create();
        await _personService.AddPersonAsync(personToAdd);
        
        var result = await _personService.DeletePersonAsync(Guid.NewGuid());
        
        Assert.False(result);
    }

    [Fact]
    public async Task DeletePerson_PersonIdExistsInTheList_DeletesPersonAndReturnsTrue()
    {
        var personToAdd = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test1@test.com").Create();
        var person1 = await _personService.AddPersonAsync(personToAdd);
        
        var result = await _personService.DeletePersonAsync(person1.PersonId);
        
        Assert.True(result);
        var person = await _personService.GetPersonByIdAsync(person1.PersonId);
        Assert.Null(person);
    }
    
    #endregion
}