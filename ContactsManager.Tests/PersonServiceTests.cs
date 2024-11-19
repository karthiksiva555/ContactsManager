using AutoFixture;
using ContactsManager.Application.DTOs;
using ContactsManager.Application.Services;
using ContactsManager.Core.Entities;
using ContactsManager.Core.Enums;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace ContactsManager.Tests;

public class PersonServiceTests
{
    private readonly CountryService _countryService;
    private readonly PersonService _personService;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Fixture _fixture;

    public PersonServiceTests(ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture();
        
        DbContextMock<ContactsDbContext> dbContextMock = new(new DbContextOptionsBuilder<ContactsDbContext>().Options);
        
        dbContextMock.CreateDbSetMock(db => db.Countries, new List<Country>().AsQueryable());
        dbContextMock.CreateDbSetMock(db => db.Persons, new List<Person>().AsQueryable());
        
        _countryService = new CountryService(null);
        _personService = new PersonService(null);
        _testOutputHelper = testOutputHelper;
    }

    #region Private Methods

    private static string[] GetPersonNames(IList<PersonResponse> persons)
    {
        var personNames = from person in persons select person.PersonName;
        return personNames as string[] ?? personNames.ToArray();
    }

    private static int GetAgeFromDateOfBirth(DateTime? dateOfBirth)
    {
        if (!dateOfBirth.HasValue)
        {
            return 0;
        }
        
        var age = DateTime.UtcNow.Year - dateOfBirth.Value.Year;
        if(dateOfBirth.Value.DayOfYear > DateTime.UtcNow.DayOfYear)
            age--;

        return age;
    }
    
    #endregion Private Methods
    
    #region AddPerson

    [Fact]
    public async Task AddPersonAsync_InputIsNull_ThrowsArgumentNullException()
    {
        Func<Task> action = async () => await _personService.AddPersonAsync(null!);
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddPersonAsync_PersonNameIsNull_ThrowsArgumentException()
    {
        var personToAdd = _fixture.Build<PersonAddRequest>()
            .With(p => p.PersonName, null as string).Create();

        Func<Task> action = async () => await _personService.AddPersonAsync(personToAdd);
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddPersonAsync_EmailAddressIsNotValid_ThrowsArgumentException()
    {
        var personToAdd = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test.com").Create();

        Func<Task> action = async () => await _personService.AddPersonAsync(personToAdd);
        
        await action.Should().ThrowAsync<ArgumentException>().WithMessage("Email Address must be in valid format.");
    }

    [Fact]
    public async Task AddPersonAsync_PersonInputIsValid_ReturnsAddedPerson()
    {
        // This will let us override specific property values
        var personToAdd = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test@test.com").Create();
        
        _testOutputHelper.WriteLine("Calling the Add Person method");
        var addedPerson = await _personService.AddPersonAsync(personToAdd);

        // Unit Testing navigation props is not easy in EF Core
        // This will fail as the person entity won't fetch Country object even with .Include() on Person fetch
        //Assert.Equal(countryResponse?.CountryName, addedPerson.Country);
        addedPerson.PersonId.Should().NotBe(Guid.Empty);
        addedPerson.PersonName.Should().Be(personToAdd.PersonName);
        addedPerson.EmailAddress.Should().Be(personToAdd.EmailAddress);
        _testOutputHelper.WriteLine($"Verifying the age property is calculated correctly");
        addedPerson.Age.Should().Be(GetAgeFromDateOfBirth(personToAdd.DateOfBirth));
    }

    #endregion

    #region GetAllPersons

    [Fact]
    public async Task GetAllPersons_BeforeInitialization_ContainsEmptyList()
    {
        var persons = await _personService.GetAllPersonsAsync();

        persons.Should().BeEmpty();
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
        
        persons.Should().HaveCount(2);
        persons.Select(p => p.PersonName).Should().Contain(person1.PersonName);
        persons.Select(p => p.PersonName).Should().Contain(person2.PersonName);
    }
    
    #endregion

    #region GetPersonById

    [Fact]
    public async Task GetPersonById_NullPersonId_ThrowsArgumentNullException()
    {
        Func<Task> action = () => _personService.GetPersonByIdAsync(Guid.Empty);
        
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetPersonById_PersonListIsEmpty_ReturnsNull()
    {
        var person = await _personService.GetPersonByIdAsync(Guid.NewGuid());

        person.Should().BeNull();
    }

    [Fact]
    public async Task GetPersonById_NoMatchingPersonFound_ReturnsNull()
    {
        var personToAdd = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test@test.com").Create();
        await _personService.AddPersonAsync(personToAdd);

        var person = await _personService.GetPersonByIdAsync(Guid.NewGuid());

        person.Should().BeNull();
    }

    [Fact]
    public async Task GetPersonById_MatchingPersonFound_ReturnsMatchingPerson()
    {
        var personToAdd = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test@test.com").Create();
        var personAdded = await _personService.AddPersonAsync(personToAdd);
        
        var person = await _personService.GetPersonByIdAsync(personAdded.PersonId);

        person.Should().NotBeNull();
        person?.PersonName.Should().Be(personAdded.PersonName);
        person?.PersonId.Should().Be(personAdded.PersonId);
    }

    #endregion
    
    #region GetFilteredPersons

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("InvalidPropertyName")]
    public async Task GetFilteredPersons_InvalidSearchByValue_ThrowsArgumentException(string searchBy)
    {
        Func<Task> action = () => _personService.GetFilteredPersonsAsync(searchBy, "any");
        
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetFilteredPersons_PersonListIsEmpty_ReturnsEmptyList()
    {
        // Person list is empty in the beginning
        var filteredPersons = await _personService.GetFilteredPersonsAsync("PersonName", "Si");
        
        filteredPersons.Should().BeEmpty();
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
        
        filteredPersons.Should().HaveCount(3);
        var personNames = GetPersonNames(filteredPersons);
        personNames.Should().Contain(person1.PersonName);
        personNames.Should().Contain(person2.PersonName);
        personNames.Should().Contain(person3.PersonName);
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
        
        filteredPersons.Should().HaveCount(2);
        var personNames = GetPersonNames(filteredPersons);
        personNames.Should().Contain(person1.PersonName);
        personNames.Should().Contain(person2.PersonName);
    }
    
    #endregion

    #region GetSortedPersons

    [Fact]
    public void GetSortedPersons_EmptyPersonsList_ReturnsEmptyList()
    {
        List<PersonResponse> persons = [];

        var sortedPersons = _personService.GetSortedPersons(persons, "PersonName", SortOrder.Asc);
        
        sortedPersons.Should().BeEmpty();
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
        
        var action = () => _personService.GetSortedPersons(persons, sortBy, SortOrder.Asc);

        action.Should().Throw<ArgumentException>();
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
        
        sortedPersons.Should().HaveCount(3);
        sortedPersons.ElementAt(0).PersonName.Should().Be("Lakshmana");
        sortedPersons.ElementAt(1).PersonName.Should().Be("Rama");
        sortedPersons.ElementAt(2).PersonName.Should().Be("Seetha");
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
        
        sortedPersons.Should().HaveCount(3);
        sortedPersons.ElementAt(0).PersonName.Should().Be("Seetha");
        sortedPersons.ElementAt(1).PersonName.Should().Be("Rama");
        sortedPersons.ElementAt(2).PersonName.Should().Be("Lakshmana");
    }
    
    #endregion

    #region UpdatePerson

    [Fact]
    public async Task UpdatePerson_PersonUpdateRequestIsNull_ThrowsArgumentNullException()
    {
        Func<Task> action = async () => await _personService.UpdatePersonAsync(null!);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdatePerson_PersonIdIsNull_ThrowsArgumentException()
    {
        var personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
            .With(p => p.EmailAddress, "test@test.com").Create();
        
        Func<Task> action = async () => await _personService.UpdatePersonAsync(personUpdateRequest);
        
        await action.Should().ThrowAsync<ArgumentException>();
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
        
        var personToUpdate = _fixture.Build<PersonUpdateRequest>().With(p => p.EmailAddress, "test3@test.com").Create();

        Func<Task> action = async () => await _personService.UpdatePersonAsync(personToUpdate);
        
        await action.Should().ThrowAsync<ArgumentException>().WithMessage(@"Invalid argument supplied (Parameter 'PersonId')");
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
        
        var personToUpdate = _fixture.Build<PersonUpdateRequest>()
            .With(p => p.PersonId, person1.PersonId)
            .With(p => p.PersonName, "Updated Name")
            .With(p => p.EmailAddress, "test3@test.com").Create();
        
        var updatedPerson = await _personService.UpdatePersonAsync(personToUpdate);
        
        updatedPerson.PersonName.Should().Be(personToUpdate.PersonName);
        var person = await _personService.GetPersonByIdAsync(person1.PersonId);
        person.Should().NotBeNull();
        person?.PersonName.Should().Be(personToUpdate.PersonName);
    }
    
    #endregion

    #region DeletePerson

    [Fact]
    public async Task DeletePerson_PersonIdIsInvalid_ThrowsArgumentException()
    {
        Func<Task> action = async () => await _personService.DeletePersonAsync(Guid.Empty);
        
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task DeletePerson_PersonIdDoesNotExistInList_ReturnsFalse()
    {
        var personToAdd = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test1@test.com").Create();
        await _personService.AddPersonAsync(personToAdd);
        
        var result = await _personService.DeletePersonAsync(Guid.NewGuid());
        
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeletePerson_PersonIdExistsInTheList_DeletesPersonAndReturnsTrue()
    {
        var personToAdd = _fixture.Build<PersonAddRequest>()
            .With(p => p.EmailAddress, "test1@test.com").Create();
        var person1 = await _personService.AddPersonAsync(personToAdd);
        
        var result = await _personService.DeletePersonAsync(person1.PersonId);
        
        result.Should().BeTrue();
        var person = await _personService.GetPersonByIdAsync(person1.PersonId);
        person.Should().BeNull();
    }
    
    #endregion
}