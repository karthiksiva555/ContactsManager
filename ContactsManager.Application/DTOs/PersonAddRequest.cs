using ContactsManager.Core.Entities;
using ContactsManager.Core.Enums;

namespace ContactsManager.Application.DTOs;

public class PersonAddRequest
{
    public string PersonName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public Gender? Gender { get; set; }

    public string? EmailAddress { get; set; }

    public Guid? CountryId { get; set; }
}

public static class PersonAddRequestExtensions
{
    public static Person ToPerson(this PersonAddRequest personAddRequest)
    {
        return new Person(){ PersonName = personAddRequest.PersonName, CountryId = personAddRequest.CountryId, EmailAddress = personAddRequest.EmailAddress, DateOfBirth = personAddRequest.DateOfBirth, Gender = personAddRequest.Gender};
    }
}