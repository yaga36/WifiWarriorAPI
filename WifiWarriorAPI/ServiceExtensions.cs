using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;

namespace WifiWarriorAPI;

public static class ServiceExtensions
{
    public static void ConfigureIdentity(this IServiceCollection services)
    {
        var builder = services.AddIdentityCore<Users>(x => x.User.RequireUniqueEmail = true);

        builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
        builder.AddEntityFrameworkStores<ApiDbContext>().AddDefaultTokenProviders();
    }

    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JWT");
        var key = configuration["JWT:Key"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        
        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException("JWT:Key is not set");
        
        if (string.IsNullOrEmpty(issuer))
            throw new InvalidOperationException("JWT:Issuer is not set");
        
        if (string.IsNullOrEmpty(audience))
            throw new InvalidOperationException("JWT:Audience is not set");
        
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };
        });
    }
}