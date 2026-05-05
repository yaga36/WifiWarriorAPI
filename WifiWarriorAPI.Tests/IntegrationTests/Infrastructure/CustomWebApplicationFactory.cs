using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;

namespace WifiWarriorAPI.Tests.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"WifiWarriorAPITests-{Guid.NewGuid()}";
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
        Environment.SetEnvironmentVariable("JWT__Key", TestConstants.JwtSecretKey);
        Environment.SetEnvironmentVariable("JWT__Issuer", TestConstants.JwtIssuer);
        Environment.SetEnvironmentVariable("JWT__Audience", TestConstants.JwtAudience);
        Environment.SetEnvironmentVariable("JWT__LifeTime", TestConstants.JwtLifeTimeMinutes);
        Environment.SetEnvironmentVariable("Cors__AllowedOrigins__0", "http://localhost");
        
        return base.CreateHost(builder);
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        
        builder.ConfigureServices((context, services) =>
        {
            var key = context.Configuration["JWT:Key"];
            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException("Missing JWT:Key");
            
            var dbContextDescriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApiDbContext>));

            if (dbContextDescriptor is not null)
                services.Remove(dbContextDescriptor);

            services.RemoveAll<DbContextOptions<ApiDbContext>>();
            services.RemoveAll<ApiDbContext>();
            services.RemoveAll<IDbContextOptionsConfiguration<ApiDbContext>>();
            
            services.AddDbContext<ApiDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
            
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Addresses.RemoveRange(db.Addresses);
            db.ConnectionInformation.RemoveRange(db.ConnectionInformation);
            db.WifiLoginDetails.RemoveRange(db.WifiLoginDetails);
            db.Venues.RemoveRange(db.Venues);
            db.ConnectionTypes.RemoveRange(db.ConnectionTypes);
            db.SaveChanges();
            
            db.Venues.Add(new Venue
            {
                Id = 1,
                Name = "Seed Venue",
                CreatedDate = DateTime.UtcNow,
            });

            db.ConnectionTypes.AddRange(
                new ConnectionType
                {
                    Id = 1,
                    Name = "Open",
                    CreatedDate = DateTime.UtcNow
                },
                new ConnectionType
                {
                    Id = 2,
                    Name = "Password",
                    CreatedDate = DateTime.UtcNow
                },
                new ConnectionType
                {
                    Id = 3,
                    Name = "Login",
                    CreatedDate = DateTime.UtcNow
                });

            db.ConnectionInformation.Add(new ConnectionInformation
            {
                Id = 1,
                ConnectionTypeId = 1,
                CreatedDate = DateTime.UtcNow
            });

            db.Addresses.Add(new Address
            {
                Id = 1,
                VenueId = 1,
                AddressLine1 = "Seed Address",
                Area = "Seed Area",
                Postcode = "ZZ11ZZ",
                ConnectionInformationId = 1,
                CreatedDate = DateTime.UtcNow
            });
            
            db.SaveChanges();
        });
    }
}