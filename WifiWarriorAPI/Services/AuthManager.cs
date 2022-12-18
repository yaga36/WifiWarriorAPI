using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WifiWarriorAPI.Models;

namespace WifiWarriorAPI.Services;

public class AuthManager : IAuthManager
{

    private readonly UserManager<Users> _userManager;
    private readonly IConfiguration _configuration;
    private Users _users;
    
    public AuthManager(UserManager<Users> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<bool> ValidateUser(LoginInfo loginUser)
    {
        _users = await _userManager.FindByNameAsync(loginUser.Email);

        return (_users != null && await _userManager.CheckPasswordAsync(_users, loginUser.Password));
    }

    public async Task<string> CreateToken()
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var jwtSettings = _configuration.GetSection("JWT");

        var token = new JwtSecurityToken(
            issuer: jwtSettings.GetSection("Issuer").Value,
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("LifeTime").Value)),
            signingCredentials: signingCredentials,
            audience: "https://localhost:7028"
        );

        return token;

    }

    private async Task<List<Claim>> GetClaims()
    {

        var claims = new List<Claim>
        {
            new Claim("name", _users.UserName)
        };

        var roles = await _userManager.GetRolesAsync(_users);

        foreach (var role in roles)
        {
            claims.Add(new Claim("roles", role));
        }
        
        return claims;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = Environment.GetEnvironmentVariable("JWT_WW");
        var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }
}