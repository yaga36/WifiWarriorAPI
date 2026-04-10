using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WifiWarriorAPI.Models;

/// <summary>
/// The model for users.
/// </summary>
public class Users : LoginUser
{
    /// <summary>
    /// The users first name.
    /// </summary>
    [MaxLength(50)]
    public required string FirstName { get; set; }
   
    /// <summary>
    /// The users last name.
    /// </summary>
    [MaxLength(50)]
    public required string LastName { get; set; }

    /// <summary>
    /// The users phone number.
    /// </summary>
    [DataType(DataType.PhoneNumber)] 
    public override string? PhoneNumber { get; set; }
}

/// <summary>
/// The model for login user identity data.
/// </summary>
public class LoginUser : IdentityUser
{
    /// <summary>
    /// The users email address.
    /// </summary>
    [DataType(DataType.EmailAddress)]
    public override string? Email { get; set; }
}
