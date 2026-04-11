using System.ComponentModel.DataAnnotations;

namespace WifiWarriorAPI.Models.Dtos.Users;

/// <summary>
/// Represents the request to update a new user.
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// The users first name.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public required string FirstName { get; init; }
    
    /// <summary>
    /// The users last name.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public required string LastName { get; init; }
    
    /// <summary>
    /// The users email.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public required string Email { get; init; }
    
    /// <summary>
    /// The optional phone number.
    /// </summary>
    [Phone]
    public string? PhoneNumber { get; init; } = null;
}