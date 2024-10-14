using ContactsManager.Core.Enums;

namespace ContactsManager.Core.Entities;

public class Person
{
    public Guid PersonId { get; set; }

    public string PersonName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public Gender? Gender { get; set; }

    public string? EmailAddress { get; set; }

    public Guid? CountryId { get; set; }
}