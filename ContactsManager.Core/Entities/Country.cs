namespace ContactsManager.Core.Entities;

/// <summary>
/// Domain model for Country object
/// </summary>
public class Country
{
    public Guid CountryId { get; set; }

    public string? CountryName { get; set; }
}