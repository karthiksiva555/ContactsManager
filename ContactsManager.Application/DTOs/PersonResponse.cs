using ContactsManager.Core.Entities;
using ContactsManager.Core.Enums;

namespace ContactsManager.Application.DTOs;

/// <summary>
/// This is a DTO class that holds Age, Country as calculated fields
/// </summary>
public class PersonResponse
{
    public Guid PersonId { get; set; }

    public string PersonName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? EmailAddress { get; set; }
    
    public Guid? CountryId { get; set; }
    
    public string? Country { get; set; }

    public int? Age { get; set; }
}

public static class PersonResponseExtensions
{
    public static PersonResponse ToPersonResponse(this Person person)
    {
        var age = person.DateOfBirth.HasValue ? DateTime.UtcNow.Year - person.DateOfBirth?.Year: 0;
        if(person.DateOfBirth?.DayOfYear > DateTime.UtcNow.DayOfYear)
            age--;
        return new PersonResponse() { PersonId = person.PersonId, Age = age, DateOfBirth = person.DateOfBirth, PersonName = person.PersonName, EmailAddress = person.EmailAddress, Gender = person.Gender.ToString(), CountryId = person.CountryId };
    }

    public static PersonUpdateRequest ToPersonUpdateRequest(this PersonResponse person)
    {
        return new PersonUpdateRequest() { PersonId = person.PersonId, PersonName = person.PersonName, EmailAddress = person.EmailAddress, Gender = person.Gender, Country = person.Country, DateOfBirth = person.DateOfBirth, CountryId = person.CountryId };
    }
}