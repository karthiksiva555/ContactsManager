using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ContactsManager.Core.Entities.Identity;

/// <summary>
/// The user class that will be extending IdentityUser and specific to this application.
/// </summary>
public class User : IdentityUser<Guid>
{
    [StringLength(50)]
    public string FullName { get; set; } = string.Empty;
}