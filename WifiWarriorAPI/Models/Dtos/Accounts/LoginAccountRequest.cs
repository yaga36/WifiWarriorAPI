using System.ComponentModel.DataAnnotations;

namespace WifiWarriorAPI.Models.Dtos.Accounts;

/// <summary>
/// A DTO used for the login request.
/// </summary>
public class LoginAccountRequest
{
    /// <summary>
    /// The user email for logging in.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public required string Email { get; init; }
    
    /// <summary>
    /// The password for logging in.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Password { get; init; }
}