using ContactsManager.Core.Entities;
using ContactsManager.Core.Interfaces;

namespace ContactsManager.Infrastructure.Repositories;

public class PersonRepository : IPersonRepository
{
    public Task<Person> AddPersonAsync(Person personToAdd)
    {
        throw new NotImplementedException();
    }

    public Task<IList<Person>> GetAllPersonsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Person?> GetPersonByIdAsync(Guid personId)
    {
        throw new NotImplementedException();
    }

    public Task<Person> UpdatePersonAsync(Person personToUpdate)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeletePersonAsync(Guid personId)
    {
        throw new NotImplementedException();
    }
}