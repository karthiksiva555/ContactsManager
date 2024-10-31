using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.Entities;

/// <summary>
/// Domain model for Country object
/// </summary>
public class Country
{
    [Key]
    public Guid CountryId { get; set; }

    [StringLength(50)]
    public string? CountryName { get; set; }
}