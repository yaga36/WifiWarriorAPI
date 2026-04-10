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
    
    public AuthManager(UserManager<Users> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<Users?> ValidateUser(LoginInfo loginUser)
    {
        var user = await _userManager.FindByNameAsync(loginUser.Email);

        if (user is null)
            return null;
        
        var valid = await _userManager.CheckPasswordAsync(user, loginUser.Password);
        return valid ? user : null;
    }

    public async Task<string> CreateToken(Users user)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims(user);
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var jwtSettings = _configuration.GetSection("JWT");

        var token = new JwtSecurityToken(
            issuer: jwtSettings.GetSection("Issuer").Value,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("LifeTime").Value)),
            signingCredentials: signingCredentials,
            audience: _configuration["JWT:Audience"]
        );

        return token;

    }

    private async Task<List<Claim>> GetClaims(Users user)
    {
        var claims = new List<Claim>
        {
            new("name", user.UserName ?? user.Email)
        };

        var roles = await _userManager.GetRolesAsync(user);

        foreach (var role in roles)
        {
            claims.Add(new Claim("roles", role));
        }
        
        return claims;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = _configuration["JWT:Key"];
        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException("JWT:Key is not set");
        
        var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }
}