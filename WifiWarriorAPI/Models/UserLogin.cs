using System.ComponentModel.DataAnnotations;

namespace WifiWarriorAPI.Models;

/// <summary>
/// User Information.
/// </summary>
public class UserInfo : LoginInfo
{
    /// <summary>
    /// First Name.
    /// </summary>
    public required string FirstName { get; set; }
   
    /// <summary>
    /// Last Name.
    /// </summary>
    public required string LastName { get; set; }
    
    /// <summary>
    /// Phone Number.
    /// </summary>
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }
}

/// <summary>
/// The model for login information.
/// </summary>
public class LoginInfo
{
    /// <summary>
    /// The users Email.
    /// </summary>
    [DataType(DataType.EmailAddress)]
    public required string Email { get; set; }

    /// <summary>
    /// The users password.
    /// </summary>
    [DataType(DataType.Password)]
    [StringLength(15, ErrorMessage = "Your password is limited to {2} and {1} character", MinimumLength = 1)]
    public required string Password { get; set; }  
}