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

    /// <inheritdoc/>
    public PersonResponse AddPerson(PersonAddRequest personToAdd)
    {
        if(personToAdd is null)
        {
            throw new ArgumentNullException(nameof(personToAdd));
        }

        if(string.IsNullOrEmpty(personToAdd.PersonName))
        {
            throw new ArgumentException(nameof(personToAdd.PersonName));
        }

        var person = personToAdd.ToPerson();
        person.PersonId = Guid.NewGuid();
        
        _persons.Add(person);

        PersonResponse? personResponse = person.ToPersonResponse();
        personResponse.Country = person.CountryId.HasValue ? _countryService.GetCountryById(person.CountryId.Value) : null;

        return personResponse;
    }

    /// <inheritdoc/>
    public IList<PersonResponse> GetAllPersons()
    {
        List<PersonResponse> persons = [];
        persons.AddRange(from person in _persons select person.ToPersonResponse());
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
        return person?.ToPersonResponse();
    }
}