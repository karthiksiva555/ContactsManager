using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ContactsManager.Core.Enums;

namespace ContactsManager.Core.Entities;

public class Person
{
    [Key]
    [Column("person_id")]
    public Guid PersonId { get; set; }

    [StringLength(50)]
    [Column("person_name")]
    public string PersonName { get; set; } = string.Empty;

    [Column("date_of_birth")]
    public DateTime? DateOfBirth { get; set; }

    [Column("gender")]
    public Gender? Gender { get; set; }

    [StringLength(50)]
    [Column("email_address")]
    public string? EmailAddress { get; set; }

    [Column("country_id")]
    public Guid? CountryId { get; set; }
    
    public Country? Country { get; set; }
}