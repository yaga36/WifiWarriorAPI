using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WifiWarriorAPI.Tests.IntegrationTests.Infrastructure;

public static class TestHelpers
{
    /// <summary>
    /// Generates a JWT for testing purposes using the specified role.
    /// </summary>
    /// <param name="role">
    /// The role to include in the token's claims.
    /// </param>
    /// <returns>
    /// A signed JSON Web Token (JWT) string containing the specified role.
    /// </returns>
    public static string CreateTestToken(string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "test-user-id"),
            new Claim(ClaimTypes.Name, "test-user"),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestConstants.JwtSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: TestConstants.JwtIssuer,
            audience: TestConstants.JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}