using System.ComponentModel.DataAnnotations;
using ContactsManager.Application.DTOs;
using ContactsManager.Application.Interfaces;
using ContactsManager.Core.Entities;

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

    /// <inheritdoc/>
    public PersonResponse AddPerson(PersonAddRequest personToAdd)
    {
        ArgumentNullException.ThrowIfNull(personToAdd);

        // Model Validation
        ValidationContext validationContext = new(personToAdd);
        List<ValidationResult> validationResults = [];
        var isValid = Validator.TryValidateObject(personToAdd, validationContext, validationResults, true);
        if(!isValid)
        {
            // Get all the error messages
            var errorMessages = from result in validationResults select result.ErrorMessage;
            throw new ArgumentException(string.Join(",", errorMessages));
        }

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
}