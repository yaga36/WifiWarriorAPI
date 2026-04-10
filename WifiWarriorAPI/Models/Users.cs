using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WifiWarriorAPI.Models;

public class Users : LoginUser
{
    public string FirstName { get; set; }
   
    public string LastName { get; set; }

    [DataType(DataType.PhoneNumber)] 
    public override string? PhoneNumber { get; set; }
}

public class LoginUser : IdentityUser
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public override string Email { get; set; }
}
