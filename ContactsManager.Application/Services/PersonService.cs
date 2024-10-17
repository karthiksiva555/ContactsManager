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
    private readonly IList<Person> _persons;
    private readonly ICountryService _countryService;

    public PersonService(ICountryService countryService)
    {
        _persons = [];
        _countryService = countryService;
    }

    private PersonResponse GetPersonResponse(Person person)
    {
        PersonResponse? personResponse = person.ToPersonResponse();
        personResponse.Country = person.CountryId.HasValue ? _countryService.GetCountryById(person.CountryId.Value) : null;

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
        
        _persons.Add(person);
        
        return GetPersonResponse(person);
    }

    /// <inheritdoc/>
    public IList<PersonResponse> GetAllPersons()
    {
        List<PersonResponse> persons = [];
        persons.AddRange(from person in _persons select GetPersonResponse(person));
        return persons;
    }

    /// <inheritdoc/>
    public PersonResponse? GetPersonById(Guid personId)
    {
        if(personId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(personId));
        }

        var person = _persons.FirstOrDefault(p => p.PersonId == personId);
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
        
        var existingPerson = _persons.FirstOrDefault(p => p.PersonId == personToUpdate.PersonId);
        if (existingPerson == null)
        {
            throw new ArgumentException("Invalid argument supplied", nameof(personToUpdate.PersonId));
        }
        
        existingPerson.PersonName = personToUpdate.PersonName;
        existingPerson.CountryId = personToUpdate.CountryId;
        existingPerson.EmailAddress = personToUpdate.EmailAddress;
        existingPerson.Gender = personToUpdate.Gender;
        existingPerson.DateOfBirth = personToUpdate.DateOfBirth;

        return existingPerson.ToPersonResponse();
    }
}