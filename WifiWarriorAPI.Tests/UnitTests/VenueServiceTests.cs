using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Venues;
using WifiWarriorAPI.Services;
using Xunit;

namespace WifiWarriorAPI.Tests.UnitTests;

public class VenueServiceTests
{
    private static VenueService CreateService(ApiDbContext context, Mock<ICredentialsProtector>? protector = null) =>
        new(context, (protector ?? CreateProtectorMock()).Object);

    private static Mock<ICredentialsProtector> CreateProtectorMock()
    {
        var protector = new Mock<ICredentialsProtector>();
        protector.Setup(x => x.Encrypt(It.IsAny<string>())).Returns((string value) => $"enc:{value}");
        return protector;
    }

    private static ApiDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        
        return new ApiDbContext(options);
    }

    [Fact]
    public async Task GetVenueByIdAsync_ShouldReturnVenue_WhenExists()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        context.Venues.Add(new Venue { Name = "Seed Venue", CreatedDate =  DateTime.UtcNow });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = CreateService(context);
        var id = context.Venues.Single().Id;
        
        // Act
        var result = await service.GetVenueByIdAsync(id, TestContext.Current.CancellationToken);
        
        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Seed Venue");
    }
    
    [Fact]
    public async Task GetVenueByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var service = CreateService(context);
        
        // Act
        var result = await service.GetVenueByIdAsync(999, TestContext.Current.CancellationToken);
        
        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetVenueSetupByIdAsync_ShouldReturnAggregate_WhenSetupExists()
    {
        // Arrange
        const string encryptedPassword = "enc:supersecret";
        var protector = CreateProtectorMock();
        protector.Setup(x => x.Decrypt(encryptedPassword)).Returns("supersecret");

        await using var context = CreateContext(Guid.NewGuid().ToString());
        context.ConnectionTypes.Add(new ConnectionType { Id = 2, Name = "Password", CreatedDate = DateTime.UtcNow });
        context.Venues.Add(new Venue { Id = 1, Name = "Venue 1", CreatedDate = DateTime.UtcNow });
        context.WifiLoginDetails.Add(new WifiLoginDetails
        {
            Id = 1,
            Ssid = "GuestWifi",
            EncryptedPassword = encryptedPassword,
            CreatedDate = DateTime.UtcNow
        });
        context.ConnectionInformation.Add(new ConnectionInformation
        {
            Id = 1,
            ConnectionTypeId = 2,
            WifiLoginDetailsId = 1,
            CreatedDate = DateTime.UtcNow
        });
        context.Addresses.Add(new Address
        {
            Id = 1,
            VenueId = 1,
            AddressLine1 = "1 High Street",
            Postcode = "AB12CD",
            ConnectionInformationId = 1,
            CreatedDate = DateTime.UtcNow
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = CreateService(context, protector);

        // Act
        var result = await service.GetVenueSetupByIdAsync(1, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Venue.Name.Should().Be("Venue 1");
        result.Address.AddressLine1.Should().Be("1 High Street");
        result.Connection.ConnectionTypeName.Should().Be("Password");
        result.WifiDetails.Should().NotBeNull();
        result.WifiDetails!.Password.Should().Be("supersecret");
        protector.Verify(x => x.Decrypt(encryptedPassword), Times.Once);
    }

    [Fact]
    public async Task GetAllVenueSetupsAsync_ShouldReturnAggregates_WhenSetupsExist()
    {
        // Arrange
        const string encryptedPassword = "enc:supersecret";
        var protector = CreateProtectorMock();
        protector.Setup(x => x.Decrypt(encryptedPassword)).Returns("supersecret");

        await using var context = CreateContext(Guid.NewGuid().ToString());
        context.ConnectionTypes.AddRange(
            new ConnectionType { Id = 1, Name = "Open", CreatedDate = DateTime.UtcNow },
            new ConnectionType { Id = 2, Name = "Password", CreatedDate = DateTime.UtcNow });
        context.Venues.AddRange(
            new Venue { Id = 1, Name = "Venue 1", CreatedDate = DateTime.UtcNow },
            new Venue { Id = 2, Name = "Venue 2", CreatedDate = DateTime.UtcNow });
        context.WifiLoginDetails.Add(new WifiLoginDetails
        {
            Id = 1,
            Ssid = "GuestWifi",
            EncryptedPassword = encryptedPassword,
            CreatedDate = DateTime.UtcNow
        });
        context.ConnectionInformation.AddRange(
            new ConnectionInformation { Id = 1, ConnectionTypeId = 1, CreatedDate = DateTime.UtcNow },
            new ConnectionInformation { Id = 2, ConnectionTypeId = 2, WifiLoginDetailsId = 1, CreatedDate = DateTime.UtcNow });
        context.Addresses.AddRange(
            new Address { Id = 1, VenueId = 1, AddressLine1 = "1 High Street", Postcode = "AB12CD", ConnectionInformationId = 1, CreatedDate = DateTime.UtcNow },
            new Address { Id = 2, VenueId = 2, AddressLine1 = "2 High Street", Postcode = "BC23DE", ConnectionInformationId = 2, CreatedDate = DateTime.UtcNow });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = CreateService(context, protector);

        // Act
        var result = await service.GetAllVenueSetupsAsync(TestContext.Current.CancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.Select(x => x.Venue.Name).Should().BeEquivalentTo("Venue 1", "Venue 2");
        result.Single(x => x.Venue.Name == "Venue 1").WifiDetails.Should().BeNull();
        result.Single(x => x.Venue.Name == "Venue 2").WifiDetails!.Password.Should().Be("supersecret");
        protector.Verify(x => x.Decrypt(encryptedPassword), Times.Once);
    }

    [Fact]
    public async Task GetVenueSetupByIdAsync_ShouldReturnNull_WhenSetupDoesNotExist()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        context.Venues.Add(new Venue { Id = 1, Name = "Venue 1", CreatedDate = DateTime.UtcNow });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = CreateService(context);

        // Act
        var result = await service.GetVenueSetupByIdAsync(1, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateVenueAsync_ShouldPersistVenue()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var service = CreateService(context);
        
        // Act
        var created = await service.CreateVenueAsync(
            new CreateVenueRequest { Name = "New Venue" },
            TestContext.Current.CancellationToken);
        
        // Assert
        created.Should().NotBeNull();
        context.Venues.Count().Should().Be(1);
    }
    
    [Fact]
    public async Task GetAllVenuesAsync_ShouldReturnVenues_WhenExists()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        context.Venues.AddRange(
            new Venue { Name = "Seed Venue 1", CreatedDate = DateTime.UtcNow },
            new Venue { Name = "Seed Venue 2", CreatedDate = DateTime.UtcNow }
        );
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var service = CreateService(context);
        
        // Act
        var result = await service.GetAllVenuesAsync(TestContext.Current.CancellationToken);
        
        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.Select(v => v.Name).Should().BeEquivalentTo("Seed Venue 1", "Seed Venue 2");
    }
    
    [Fact]
    public async Task GetAllVenuesAsync_ShouldBeEmpty_WhenNotExists()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var service = CreateService(context);
        
        // Act
        var result = await service.GetAllVenuesAsync(TestContext.Current.CancellationToken);
        
        // Assert
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task UpdateVenueAsync_ShouldReturnTrue_WhenExists()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var arrangeContext = CreateContext(dbName);
        arrangeContext.Venues.Add(
            new Venue { Name = "Seed Venue", CreatedDate = DateTime.UtcNow }
        );
        await arrangeContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        await using var actContext = CreateContext(dbName);
        var service = CreateService(actContext);
        var id = actContext.Venues.Single().Id;
        
        var updatedVenue = new UpdateVenueRequest
        {
            Name = "Updated Venue",
        };
        
        var result = await service.UpdateVenueAsync(id, updatedVenue, TestContext.Current.CancellationToken);
        result.Should().BeTrue();
        
        // Assert
        await using var assertContext = CreateContext(dbName);
        assertContext.Venues.First().Name.Should().Be("Updated Venue");
    }
    
    [Fact]
    public async Task UpdateVenueAsync_ShouldReturnFalse_WhenNotExists()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var service = CreateService(context);
        
        var updatedVenue = new UpdateVenueRequest
        {
            Name = "Updated Venue",
        };
        
        // Act
        var result = await service.UpdateVenueAsync(999, updatedVenue, TestContext.Current.CancellationToken);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task DeleteVenueAsync_ShouldReturnTrue_WhenExists()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var arrangeContext = CreateContext(dbName);
        arrangeContext.Venues.Add(
            new Venue { Name = "Seed Venue", CreatedDate = DateTime.UtcNow }
        );
        await arrangeContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        await using var actContext = CreateContext(dbName);
        var service = CreateService(actContext);
        var id = actContext.Venues.Single().Id;
        var result = await service.DeleteVenueAsync(id, TestContext.Current.CancellationToken);
        result.Should().BeTrue();
        
        // Assert
        await using var assertContext = CreateContext(dbName);
        assertContext.Venues.Count().Should().Be(0);
    }
    
    [Fact]
    public async Task DeleteVenueAsync_ShouldReturnFalse_WhenNotExists()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var service = CreateService(context);
        
        // Act
        var result = await service.DeleteVenueAsync(999, TestContext.Current.CancellationToken);
        
        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CreateVenueSetupAsync_ShouldCreateAllEntities_WhenConnectionRequiresCredentials()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        context.ConnectionTypes.Add(new ConnectionType { Id = 2, Name = "Password", CreatedDate = DateTime.UtcNow });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var protector = CreateProtectorMock();
        var service = CreateService(context, protector);
        var request = new CreateVenueSetupRequest
        {
            VenueName = "Cafe 123",
            Address = new CreateVenueSetupAddressRequest
            {
                AddressLine1 = "1 High Street",
                Postcode = "AB12CD",
                Latitude = 51.5,
                Longitude = -0.12
            },
            Connection = new CreateVenueSetupConnectionRequest
            {
                ConnectionTypeId = 2,
                Ssid = "GuestWifi",
                Password = "supersecret"
            }
        };

        // Act
        var result = await service.CreateVenueSetupAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Venue.Name.Should().Be("Cafe 123");
        result.Value.Address.VenueId.Should().Be(result.Value.Venue.Id);
        result.Value.Connection.ConnectionTypeId.Should().Be(2);
        result.Value.WifiDetails.Should().NotBeNull();
        result.Value.WifiDetails!.Ssid.Should().Be("GuestWifi");
        result.Value.WifiDetails.Password.Should().Be("supersecret");

        context.Venues.Should().HaveCount(1);
        context.WifiLoginDetails.Should().HaveCount(1);
        context.ConnectionInformation.Should().HaveCount(1);
        context.Addresses.Should().HaveCount(1);
        context.WifiLoginDetails.Single().EncryptedPassword.Should().Be("enc:supersecret");

        protector.Verify(x => x.Encrypt("supersecret"), Times.Once);
    }

    [Fact]
    public async Task CreateVenueSetupAsync_ShouldReturnBadRequest_WhenProtectedConnectionMissingCredentials()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        context.ConnectionTypes.Add(new ConnectionType { Id = 2, Name = "Password", CreatedDate = DateTime.UtcNow });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var protector = CreateProtectorMock();
        var service = CreateService(context, protector);
        var request = new CreateVenueSetupRequest
        {
            VenueName = "Cafe 123",
            Address = new CreateVenueSetupAddressRequest
            {
                AddressLine1 = "1 High Street",
                Postcode = "AB12CD"
            },
            Connection = new CreateVenueSetupConnectionRequest
            {
                ConnectionTypeId = 2,
                Ssid = "GuestWifi"
            }
        };

        // Act
        var result = await service.CreateVenueSetupAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        context.Venues.Should().BeEmpty();
        context.ConnectionInformation.Should().BeEmpty();
        protector.Verify(x => x.Encrypt(It.IsAny<string>()), Times.Never);
    }
}