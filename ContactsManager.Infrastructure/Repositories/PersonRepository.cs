using ContactsManager.Application.RepositoryInterfaces;
using ContactsManager.Core.Entities;
using ContactsManager.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Infrastructure.Repositories;

public class PersonRepository(ContactsDbContext contactsDbContext) : IPersonRepository
{
    public async Task<Person> AddPersonAsync(Person person)
    {
        await contactsDbContext.Persons.AddAsync(person);
        await contactsDbContext.SaveChangesAsync();
        
        return person;
    }

    public async Task<IList<Person>> GetAllPersonsAsync()
    {
        return await contactsDbContext.Persons.Include(p => p.Country).ToListAsync();
    }

    public async Task<Person?> GetPersonByIdAsync(Guid personId)
    {
        return await contactsDbContext.Persons.FindAsync(personId);
    }

    public async Task<Person> UpdatePersonAsync(Person person)
    {
        await contactsDbContext.Persons
            .Where(p => p.PersonId == person.PersonId)
            .ExecuteUpdateAsync(personToUpdate => personToUpdate // Set properties those you want updated
                .SetProperty(p => p.PersonName, person.PersonName)
                .SetProperty(p => p.EmailAddress, person.EmailAddress)
                .SetProperty(p => p.Gender, person.Gender)
                .SetProperty(p => p.DateOfBirth, person.DateOfBirth)
                .SetProperty(p => p.CountryId, person.CountryId)
            );
        
        // check if you need to fetch the entity from the database from scratch
        return person;
    }

    public async Task<bool> DeletePersonAsync(Guid personId)
    {
        var rowsDeleted = await contactsDbContext.Persons.Where(p => p.PersonId == personId).ExecuteDeleteAsync();
        
        return rowsDeleted > 0;
    }
}