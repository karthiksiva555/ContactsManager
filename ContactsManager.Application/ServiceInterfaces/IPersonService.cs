using ContactsManager.Application.DTOs;
using ContactsManager.Core.Enums;

namespace ContactsManager.Application.ServiceInterfaces;

/// <summary>
/// Provides an inteface to perform CRUD operations on the Person entity.
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Receives a PersonAddRequest as input, adds it to the list of Person objects.
    /// </summary>
    /// <param name="personToAdd">The Person DTO</param>
    /// <returns>A PersonResponse object after addition is complete.</returns>
    Task<PersonResponse> AddPersonAsync(PersonAddRequest personToAdd);

    /// <summary>
    /// Returns all the Person records added prior to this call.
    /// </summary>
    /// <returns>A list of PersonResponse objects</returns>
    Task<IList<PersonResponse>> GetAllPersonsAsync();

    /// <summary>
    /// Finds and returns a person that matches the supplied person id.
    /// </summary>
    /// <param name="personId">The ID of the person to be searched for.</param>
    /// <returns>The matched person if found, null otherwise</returns>
    Task<PersonResponse?> GetPersonByIdAsync(Guid personId);
    
    /// <summary>
    /// Returns all the person objects that matches the give search string in the searchBy field.
    /// </summary>
    /// <param name="searchBy">The field to search.</param>
    /// <param name="searchString">The string to search with.</param>
    /// <returns>The person records matching the search criteria</returns>
    Task<IList<PersonResponse>> GetFilteredPersonsAsync(string searchBy, string? searchString);
    
    /// <summary>
    /// Receives a list of persons and returns the list sorted by the field, order specified.
    /// </summary>
    /// <param name="allPersons">All persons to be sorted</param>
    /// <param name="sortBy">The field to be sorted by.</param>
    /// <param name="sortOrder">The sort order option Asc for Ascending or Desc for Descending</param>
    /// <returns>The sorted list of persons</returns>
    IList<PersonResponse> GetSortedPersons(IList<PersonResponse> allPersons, string sortBy, SortOrder sortOrder);
    
    /// <summary>
    /// Updates a person record
    /// </summary>
    /// <param name="personToUpdate">The updated person information along with the person id.</param>
    /// <returns>The updated person record</returns>
    Task<PersonResponse> UpdatePersonAsync(PersonUpdateRequest personToUpdate);
    
    /// <summary>
    /// Deletes a person matching the supplied person id
    /// </summary>
    /// <param name="personId">The ID of the person to be deleted</param>
    /// <returns>True if deletion is successful, false otherwise</returns>
    Task<bool> DeletePersonAsync(Guid personId);
}