using ContactsManager.Application.DTOs;
using ContactsManager.Application.Interfaces;

namespace ContactsManager.Application.Services;

/// <inheritdoc/>
public class PersonService : IPersonService
{
    /// <inheritdoc/>
    public PersonResponse AddPerson(PersonAddRequest personToAdd)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IList<PersonResponse> GetAllPersons()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public PersonResponse? GetPersonById(Guid personId)
    {
        throw new NotImplementedException();
    }
}