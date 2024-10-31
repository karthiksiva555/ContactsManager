using System.ComponentModel.DataAnnotations;
using ContactsManager.Core.Enums;

namespace ContactsManager.Core.Entities;

public class Person
{
    [Key]
    public Guid PersonId { get; set; }

    [StringLength(50)]
    public string PersonName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public Gender? Gender { get; set; }

    [StringLength(50)]
    public string? EmailAddress { get; set; }

    public Guid? CountryId { get; set; }
    
    public Country? Country { get; set; }
}