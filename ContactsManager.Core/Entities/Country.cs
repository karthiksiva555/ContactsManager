using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactsManager.Core.Entities;

/// <summary>
/// Domain model for Country object
/// </summary>
public class Country
{
    [Key]
    [Column("country_id")]
    public Guid CountryId { get; set; }

    [StringLength(50)]
    [Column("country_name")]
    public string? CountryName { get; set; }
}