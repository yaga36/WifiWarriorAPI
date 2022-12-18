using WifiWarriorAPI.Models;

namespace WifiWarriorAPI.Services;

public interface IAuthManager
{
    Task<bool> ValidateUser(LoginInfo loginUser);
    Task<string> CreateToken();
}