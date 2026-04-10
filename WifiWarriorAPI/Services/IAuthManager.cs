using WifiWarriorAPI.Models;

namespace WifiWarriorAPI.Services;

public interface IAuthManager
{
    Task<Users?> ValidateUser(LoginInfo loginUser);
    Task<string> CreateToken(Users user);
}