using System.Reflection;
using ContactsManager.Application.DTOs;
using ContactsManager.Application.Helpers;
using ContactsManager.Application.Interfaces;
using ContactsManager.Core.Entities;
using ContactsManager.Core.Enums;

namespace ContactsManager.Application.Services;

/// <inheritdoc/>
public class PersonService : IPersonService
{
    private readonly ContactsDbContext _database;
    private readonly ICountryService _countryService;

    public PersonService(ICountryService countryService, ContactsDbContext contactsDbContext)
    {
        _countryService = countryService;
        _database = contactsDbContext;
    }

    private PersonResponse GetPersonResponse(Person person)
    {
        PersonResponse? personResponse = person.ToPersonResponse();
        var country = person.CountryId.HasValue ? _countryService.GetCountryById(person.CountryId.Value) : null;
        personResponse.Country = country?.CountryName ?? string.Empty;

        return personResponse;
    }

    private static bool PropertyContainsSearchString(string propertyName, PersonResponse person, string searchString)
    {
        var personType = person.GetType();
        var property = personType.GetProperty(propertyName);
        var propertyValue = property?.GetValue(person)?.ToString();
        return propertyValue?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false;
    }

    private static bool PropertyOfPerson(string propertyName)
    {
        var personType = typeof(Person);
        var property = personType.GetProperty(propertyName);
        return property != null;
    }
    
    /// <inheritdoc/>
    public PersonResponse AddPerson(PersonAddRequest personToAdd)
    {
        ArgumentNullException.ThrowIfNull(personToAdd);

        // Model Validation; returns void if object is valid; throws an exception otherwise
        ValidationHelper.Validate(personToAdd);

        var person = personToAdd.ToPerson();
        person.PersonId = Guid.NewGuid();
        
        // Dummy database
        // _persons.Add(person);
        
        // EF Core with SaveChanges
        // _database.Persons.Add(person);
        // _database.SaveChanges();
        
        // EF Core with Stored procedure
        var result = _database.InsertPerson(person);

        if (result == 0)
        {
            throw new Exception("Person could not be added.");
        }
        
        return GetPersonResponse(person);
    }

    /// <inheritdoc/>
    public IList<PersonResponse> GetAllPersons()
    {
        List<PersonResponse> persons = [];
        // var personsFromDb = _database.Persons.ToList();
        var personsFromDb = _database.FunctionGetAllPersons();
        persons.AddRange(from person in personsFromDb select GetPersonResponse(person));
        return persons;
    }

    /// <inheritdoc/>
    public PersonResponse? GetPersonById(Guid personId)
    {
        if(personId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(personId));
        }

        var person = _database.Persons.FirstOrDefault(p => p.PersonId == personId);
        return person!=null ? GetPersonResponse(person) : null;
    }

    /// <inheritdoc/>
    public IList<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
    {
        // If searchBy is null, empty or not a property of Person class, throw exception
        if (string.IsNullOrEmpty(searchBy) || !PropertyOfPerson(searchBy))
        {
            throw new ArgumentException("Invalid argument supplied.", nameof(searchBy));
        }
        
        // If search string is empty, return all records as result
        if (string.IsNullOrEmpty(searchString))
        {
            return GetAllPersons();
        }
        
        var allPersons = GetAllPersons();
        return allPersons.Where(person => PropertyContainsSearchString(searchBy, person, searchString)).ToList();
    }

    /// <inheritdoc/>
    public IList<PersonResponse> GetSortedPersons(IList<PersonResponse> allPersons, string sortBy, SortOrder sortOrder)
    {
        if (allPersons.Count == 0)
        {
            return allPersons;
        }

        if (string.IsNullOrEmpty(sortBy) || !PropertyOfPerson(sortBy))
        {
            throw new ArgumentException("Invalid argument supplied.", nameof(sortBy));
        }

        var property = typeof(PersonResponse).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        Func<PersonResponse, object> keySelector = p => property!.GetValue(p, null)!;
        var sortedPersons = sortOrder == SortOrder.Desc
            ? allPersons.OrderByDescending(keySelector).ToList()
            : allPersons.OrderBy(keySelector).ToList();

        return sortedPersons;
    }

    /// <exception cref="ArgumentException"></exception>
    /// <inheritdoc/>
    public PersonResponse UpdatePerson(PersonUpdateRequest personToUpdate)
    {
        ArgumentNullException.ThrowIfNull(personToUpdate);

        if (personToUpdate.PersonId == Guid.Empty)
        {
            throw new ArgumentException("Person Id cannot be blank", nameof(personToUpdate.PersonId));
        }

        // Perform all model validations
        ValidationHelper.Validate(personToUpdate);
        
        var existingPerson = _database.Persons.FirstOrDefault(p => p.PersonId == personToUpdate.PersonId);
        if (existingPerson == null)
        {
            throw new ArgumentException("Invalid argument supplied", nameof(personToUpdate.PersonId));
        }
        
        // Any change to the existing entity will mark it as EntityStateModified = true
        existingPerson.PersonName = personToUpdate.PersonName;
        existingPerson.CountryId = personToUpdate.CountryId;
        existingPerson.EmailAddress = personToUpdate.EmailAddress;
        existingPerson.Gender = Enum.Parse<Gender>(personToUpdate.Gender);
        existingPerson.DateOfBirth = personToUpdate.DateOfBirth;

        // Generates UPDATE statement for all rows with EntityStateModified = true
        _database.SaveChanges();
        
        return existingPerson.ToPersonResponse();
    }

    /// <inheritdoc/>
    public bool DeletePerson(Guid personId)
    {
        if (personId == Guid.Empty)
        {
            throw new ArgumentException("Invalid argument supplied", nameof(personId));
        }
        
        var existingPerson = _database.Persons.FirstOrDefault(p => p.PersonId == personId);

        if (existingPerson == null)
            return false;
        
        _database.Persons.Remove(existingPerson);
        _database.SaveChanges();
        
        return true;
    }
}