using System.Reflection;
using ContactsManager.Application.DTOs;
using ContactsManager.Application.Helpers;
using ContactsManager.Application.ServiceInterfaces;
using ContactsManager.Core.Entities;
using ContactsManager.Core.Enums;
using ContactsManager.Application.RepositoryInterfaces;

namespace ContactsManager.Application.Services;

/// <inheritdoc/>
public class PersonService(IPersonRepository personRepository)
    : IPersonService
{
    private static PersonResponse GetPersonResponse(Person person)
    {
        var personResponse = person.ToPersonResponse();
        personResponse.Country = person.Country?.CountryName ?? string.Empty;

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
    public async Task<PersonResponse> AddPersonAsync(PersonAddRequest personToAdd)
    {
        ArgumentNullException.ThrowIfNull(personToAdd);

        // Model Validation; returns void if object is valid; throws an exception otherwise
        ValidationHelper.Validate(personToAdd);
        
        var addedPerson = await personRepository.AddPersonAsync(personToAdd.ToPerson());
        return GetPersonResponse(addedPerson);
    }

    /// <inheritdoc/>
    public async Task<IList<PersonResponse>> GetAllPersonsAsync()
    {
        List<PersonResponse> persons = [];
        var personsFromDb = await personRepository.GetAllPersonsAsync();
        persons.AddRange(from person in personsFromDb select GetPersonResponse(person));
        return persons;
    }

    /// <inheritdoc/>
    public async Task<PersonResponse?> GetPersonByIdAsync(Guid personId)
    {
        if(personId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(personId));
        }

        var person = await personRepository.GetPersonByIdAsync(personId);
        return person!=null ? GetPersonResponse(person) : null;
    }

    /// <inheritdoc/>
    public async Task<IList<PersonResponse>> GetFilteredPersonsAsync(string searchBy, string? searchString)
    {
        // If searchBy is null, empty or not a property of Person class, throw exception
        if (string.IsNullOrEmpty(searchBy) || !PropertyOfPerson(searchBy))
        {
            throw new ArgumentException("Invalid argument supplied.", nameof(searchBy));
        }
        
        // If search string is empty, return all records as result
        if (string.IsNullOrEmpty(searchString))
        {
            return await GetAllPersonsAsync();
        }
        
        var allPersons = await GetAllPersonsAsync();
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
    public async Task<PersonResponse> UpdatePersonAsync(PersonUpdateRequest personToUpdate)
    {
        ArgumentNullException.ThrowIfNull(personToUpdate);

        if (personToUpdate.PersonId == Guid.Empty)
        {
            throw new ArgumentException("Person Id cannot be blank", nameof(personToUpdate.PersonId));
        }

        // Perform all model validations
        ValidationHelper.Validate(personToUpdate);
        
        var existingPerson = await personRepository.GetPersonByIdAsync(personToUpdate.PersonId);
        if (existingPerson == null)
        {
            throw new ArgumentException("Invalid argument supplied", nameof(personToUpdate.PersonId));
        }
        
        var person = await personRepository.UpdatePersonAsync(personToUpdate.ToPerson());
        
        // Any change to the existing entity will mark it as EntityStateModified = true
        // existingPerson.PersonName = personToUpdate.PersonName;
        // existingPerson.CountryId = personToUpdate.CountryId;
        // existingPerson.EmailAddress = personToUpdate.EmailAddress;
        // if (!string.IsNullOrEmpty(personToUpdate.Gender) &&
        //     Enum.TryParse<Gender>(personToUpdate.Gender, out var gender))
        //     existingPerson.Gender = gender;
        // existingPerson.DateOfBirth = personToUpdate.DateOfBirth;
        //
        // // Generates UPDATE statement for all rows with EntityStateModified = true
        // await contactsDbContext.SaveChangesAsync();
        
        return person.ToPersonResponse();
    }

    /// <inheritdoc/>
    public async Task<bool> DeletePersonAsync(Guid personId)
    {
        if (personId == Guid.Empty)
        {
            throw new ArgumentException("Invalid argument supplied", nameof(personId));
        }
        
        var existingPerson = await personRepository.GetPersonByIdAsync(personId);

        if (existingPerson == null)
            return false;
        
        return await personRepository.DeletePersonAsync(personId);
    }
}