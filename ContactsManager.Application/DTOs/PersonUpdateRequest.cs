using System.ComponentModel.DataAnnotations;
using ContactsManager.Core.Entities;
using ContactsManager.Core.Enums;

namespace ContactsManager.Application.DTOs;

/// <summary>
/// The data supplied to update a person record
/// </summary>
public class PersonUpdateRequest
{
    [Required(ErrorMessage = "Person Id cannot be blank")]
    public Guid PersonId { get; set; } = Guid.Empty;

    [Required]
    public string PersonName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    [EmailAddress(ErrorMessage = "Email Address must be in valid format.")]
    public string? EmailAddress { get; set; }

    public Guid? CountryId { get; set; }
    
    public string? Country { get; set; }
}

public static class PersonUpdateRequestExtensions
{
    public static Person ToPerson(this PersonUpdateRequest personUpdateRequest)
    {
        return new Person { PersonId = personUpdateRequest.PersonId, PersonName = personUpdateRequest.PersonName, EmailAddress = personUpdateRequest.EmailAddress, DateOfBirth = personUpdateRequest.DateOfBirth, Gender = Enum.Parse<Gender>(personUpdateRequest.Gender), CountryId = personUpdateRequest.CountryId };
    }
}