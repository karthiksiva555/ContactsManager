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

    public Country? Country { get; set; }

    public int Age { get; set; }
}