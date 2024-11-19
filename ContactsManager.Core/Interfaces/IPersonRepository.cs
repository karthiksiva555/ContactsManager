using ContactsManager.Core.Entities;

namespace ContactsManager.Core.Interfaces;

public interface IPersonRepository
{
    Task<Person> AddPersonAsync(Person person);

    Task<IList<Person>> GetAllPersonsAsync();

    Task<Person?> GetPersonByIdAsync(Guid personId);

    Task<Person> UpdatePersonAsync(Person personToUpdate);

    Task<bool> DeletePersonAsync(Guid personId);
}