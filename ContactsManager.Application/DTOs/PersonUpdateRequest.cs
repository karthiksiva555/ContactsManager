using System.ComponentModel.DataAnnotations;
using ContactsManager.Core.Enums;

namespace ContactsManager.Application.DTOs;

/// <summary>
/// The data supplied to update a person record
/// </summary>
public class PersonUpdateRequest
{
    [Required(ErrorMessage = "Person Id cannot be blank")]
    public Guid PersonId { get; set; }

    [Required]
    public string PersonName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public Gender? Gender { get; set; }

    [EmailAddress(ErrorMessage = "Email Address must be in valid format.")]
    public string? EmailAddress { get; set; }

    public Guid? CountryId { get; set; }
}