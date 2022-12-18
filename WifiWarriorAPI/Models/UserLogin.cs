using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WifiWarriorAPI.Models;

public class UserInfo : LoginInfo
{
    public string FirstName { get; set; }
   
    public string LastName { get; set; }
    
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }
    
    public ICollection<string> Roles { get; set; }
}

public class LoginInfo : IdentityUser
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(15, ErrorMessage = "Your password is limited to {2} and {1} character", MinimumLength = 1)]
    public string Password { get; set; }  
}