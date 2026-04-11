using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Venues;
using WifiWarriorAPI.Services;
using Xunit;

namespace WifiWarriorAPI.Tests.UnitTests;

public class VenueServiceTests
{
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

        var service = new VenueService(context);
        var id = context.Venues.Single().Id;
        
        // Act
        var result = await service.GetVenueByIdAsync(id, TestContext.Current.CancellationToken);
        
        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Seed Venue");
    }
    
    [Fact]
    public async Task GetVenueByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var service = new VenueService(context);
        
        // Act
        var result = await service.GetVenueByIdAsync(999, TestContext.Current.CancellationToken);
        
        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateVenueAsync_ShouldPersistVenue()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var service = new VenueService(context);
        
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

        var service = new VenueService(context);
        
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
        var service = new VenueService(context);
        
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
        var service = new VenueService(actContext);
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
        var service = new VenueService(context);
        
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
        var service = new VenueService(actContext);
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
        var service = new VenueService(context);
        
        // Act
        var result = await service.DeleteVenueAsync(999, TestContext.Current.CancellationToken);
        
        // Assert
        result.Should().BeFalse();
    }
}