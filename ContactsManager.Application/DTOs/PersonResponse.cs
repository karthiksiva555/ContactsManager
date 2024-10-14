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

    public Gender? Gender { get; set; }

    public string? EmailAddress { get; set; }

    public CountryResponse? Country { get; set; }

    public int? Age { get; set; }
}

public static class PersonResponseExtensions
{
    public static PersonResponse ToPersonResponse(this Person person)
    {
        var age = person.DateOfBirth?.Year - DateTime.UtcNow.Year;
        if(person.DateOfBirth?.DayOfYear > DateTime.UtcNow.DayOfYear)
            age--;
        return new PersonResponse() { PersonId = person.PersonId, Age = age, DateOfBirth = person.DateOfBirth, PersonName = person.PersonName, EmailAddress = person.EmailAddress, Gender = person.Gender};
    }
}