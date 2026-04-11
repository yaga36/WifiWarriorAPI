using System.ComponentModel.DataAnnotations;

namespace WifiWarriorAPI.Models.Dtos.Accounts;

/// <summary>
/// A DTO for registering a new account.
/// </summary>
public class RegisterAccountRequest
{
    /// <summary>
    /// The first name for registration.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public required string FirstName { get; init; }
    
    /// <summary>
    /// The last name for registration.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public required string LastName { get; init; }
    
    /// <summary>
    /// The user email for registration.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public required string Email { get; init; }
    
    /// <summary>
    /// The password for registering.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Password { get; init; }

    /// <summary>
    /// The phone number.
    /// </summary>
    [Phone]
    public string? PhoneNumber { get; init; }
}