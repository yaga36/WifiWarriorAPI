namespace WifiWarriorAPI.Models.Dtos.Users;

/// <summary>
/// The user response model.
/// </summary>
public class UserResponse
{
    /// <summary>
    /// The user identifier.
    /// </summary>
    public required string Id { get; init; }
    
    /// <summary>
    /// The users first name.
    /// </summary>
    public required string FirstName { get; init; }
    
    /// <summary>
    /// The users last name.
    /// </summary>
    public required string LastName { get; init; }
    
    /// <summary>
    /// The users email.
    /// </summary>
    public required string Email { get; init; }
    
    /// <summary>
    /// The optional phone number.
    /// </summary>
    public string? PhoneNumber { get; init; } = null;
}