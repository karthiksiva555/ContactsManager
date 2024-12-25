using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Application.DTOs;

public class RegisterUserDto
{
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(50, ErrorMessage = "Full name cannot be longer than 50 characters")]
    public string FullName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Username is required")]
    [StringLength(20, ErrorMessage = "Username cannot be longer than 20 characters")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "ConfirmPassword is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ComparePassword { get; set; } = string.Empty;
}