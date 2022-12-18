using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Configurations.Entities;
using WifiWarriorAPI.Models;

namespace WifiWarriorAPI.Data;

public class ApiDbContext: IdentityDbContext<Users>
{
    public virtual DbSet<Venue> Venues { get; set; }
    public virtual DbSet<Address> Addresses { get; set; }
    public virtual DbSet<ConnectionType> ConnectionTypes { get; set; }
    public virtual DbSet<ConnectionInformation> ConnectionInformation { get; set; }
    public virtual DbSet<Users> Users { get; set; }
    public virtual DbSet<WifiLoginDetails> WifiLoginDetails { get; set; }

    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options){ }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasOne(p => p.Venue)
                .WithMany()
                .HasForeignKey(fk => fk.VenueId);
        });
        
        modelBuilder.Entity<ConnectionInformation>(entity =>
        {
            entity.HasOne(ct => ct.ConnectionType)
                .WithOne()
                .HasForeignKey<ConnectionInformation>(ci => ci.ConnectionTypeId);

            entity.HasOne(wd => wd.WifiLoginDetails)
                .WithOne()
                .HasForeignKey<ConnectionInformation>(ci => ci.WifiLoginDetailsId);
        });
        
        modelBuilder.Entity<Users>().HasData(new Users
        {
            Id = "1",
            UserName = "TestUsername",
            PasswordHash = "UserPasswordHash",
            Password = "UserPassword",
            FirstName = "TestName",
            LastName = "TestLastName",
            Email = "Test@email.com",
            PhoneNumber = "07123456789"
        });

        var connectionType = new List<ConnectionType>();
        
        connectionType.Add(new ConnectionType
        {
            Id = 1,
            Name = "Open"
        });
        
        connectionType.Add(new ConnectionType
        {
            Id = 2,
            Name = "Password"
        });
        
        connectionType.Add(new ConnectionType
        {
            Id = 3,
            Name = "Login"
        });
        
        modelBuilder.Entity<ConnectionType>().HasData(connectionType);

        modelBuilder.Entity<WifiLoginDetails>().HasData(new WifiLoginDetails
        {
            Id = 1,
            Ssid = "SSID",
            Password = "Password",
        });

        modelBuilder.Entity<ConnectionInformation>().HasData(new ConnectionInformation
        {
            Id = 1,
            ConnectionTypeId = 2,
            WifiLoginDetailsId = 1
        });

        modelBuilder.Entity<Address>().HasData(new Address
        {
            Id = 1,
            AddressLine1 = "Address Line 1",
            AddressLine2 = "Address Line 2",
            Area = "Area",
            County = "County",
            Postcode = "Postcode",
            Latitude = 0.1,
            Longitude = 0.1,
            VenueId = 1,
            ConnectionInformationId = 1
        });
        
        modelBuilder.Entity<Venue>().HasData(new Venue
        {
            Id = 1,
            Name = "Venue Name"
        });
        
    }
}