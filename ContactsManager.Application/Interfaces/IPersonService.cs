using ContactsManager.Application.DTOs;

namespace ContactsManager.Application.Interfaces;

public interface IPersonService
{
    /// <summary>
    /// Receives a PersonAddRequest as input, adds it to the list of Person objects.
    /// </summary>
    /// <param name="personToAdd">The Person DTO</param>
    /// <returns>A PersonResponse object after addition is complete.</returns>
    PersonResponse AddPerson(PersonAddRequest personToAdd);

    /// <summary>
    /// Returns all the Person records added prior to this call.
    /// </summary>
    /// <returns>A list of PersonResponse objects</returns>
    IList<PersonResponse> GetAllPersons();

    /// <summary>
    /// Finds and returns a person that matches the supplied person id.
    /// </summary>
    /// <param name="personId">The Id of the person to be searched for.</param>
    /// <returns>The matched person if found, null otherwise</returns>
    PersonResponse? GetPersonById(Guid personId);
}