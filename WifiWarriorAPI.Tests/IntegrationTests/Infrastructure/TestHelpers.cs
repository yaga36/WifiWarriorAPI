using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Moq;
using WifiWarriorAPI.Models;

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
    
    /// <summary>
    /// Creates a mock <see cref="UserManager{Users}"/> with minimal dependencies for testing.
    /// </summary>
    /// <returns>
    /// A configured mock of <see cref="UserManager{Users}"/> suitable for unit tests.
    /// </returns>
    public static Mock<UserManager<Users>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<Users>>();
        return new Mock<UserManager<Users>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }
}