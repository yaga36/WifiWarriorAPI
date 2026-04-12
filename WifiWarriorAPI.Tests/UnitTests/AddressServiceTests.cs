using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Addresses;
using WifiWarriorAPI.Services;
using Xunit;

namespace WifiWarriorAPI.Tests.UnitTests;

/// <summary>
/// Unit tests for <see cref="AddressService"/>.
/// </summary>
public class AddressServiceTests
{
    [Fact]
    public async Task GetAllAddressesAsync_ShouldReturnMappedAddresses_WhenAddressesExist()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        SeedAddressGraph(context);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        
        var sut = new AddressService(context);

        // Act
        var result = await sut.GetAllAddressesAsync(TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result.First().AddressLine1.Should().Be("1 Test Street");
        result.First().VenueName.Should().Be("Venue A");
    }

    [Fact]
    public async Task GetAddressByIdAsync_ShouldReturnAddress_WhenExists()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var addressId = SeedAddressGraph(context);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        
        var sut = new AddressService(context);

        // Act
        var result = await sut.GetAddressByIdAsync(addressId, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(addressId);
        result.Postcode.Should().Be("TE1 1ST");
    }

    [Fact]
    public async Task GetAddressByIdAsync_ShouldReturnNull_WhenMissing()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var sut = new AddressService(context);

        // Act
        var result = await sut.GetAddressByIdAsync(999, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAddressAsync_ShouldReturnCreated_WhenValidRequest()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        context.Venues.Add(new Venue { Id = 1, Name = "Venue A", CreatedDate = DateTime.UtcNow });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        
        var sut = new AddressService(context);

        var request = new CreateAddressRequest
        {
            AddressLine1 = "10 New Street",
            AddressLine2 = "Suite 2",
            Area = "Area",
            County = "County",
            Postcode = "NW1 1AA",
            Latitude = 51.5,
            Longitude = -0.12,
            VenueId = 1
        };

        // Act
        var result = await sut.CreateAddressAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value.Should().NotBeNull();
        result.Value!.AddressLine1.Should().Be("10 New Street");
    }

    [Fact]
    public async Task UpdateAddressAsync_ShouldReturnNotFound_WhenAddressMissing()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var sut = new AddressService(context);

        var request = new UpdateAddressRequest
        {
            AddressLine1 = "Updated Street",
            Postcode = "UP1 1DT",
            VenueId = 1
        };

        // Act
        var result = await sut.UpdateAddressAsync(999, request, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task UpdateAddressAsync_ShouldReturnNoContent_WhenAddressExists()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var arrangeContext = CreateContext(dbName);
        SeedAddressGraph(arrangeContext);
        await arrangeContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await using var actContext = CreateContext(dbName);
        var sut = new AddressService(actContext);

        var targetId = await actContext.Addresses.Select(x => x.Id).SingleAsync(TestContext.Current.CancellationToken);

        var request = new UpdateAddressRequest
        {
            AddressLine1 = "Updated Street",
            AddressLine2 = "Unit 5",
            Area = "Updated Area",
            County = "Updated County",
            Postcode = "UP1 1DT",
            Latitude = 50.0,
            Longitude = -1.0,
            VenueId = 1,
            ConnectionInformationId = 1
        };

        // Act
        var result = await sut.UpdateAddressAsync(targetId, request, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        await using var assertContext = CreateContext(dbName);
        var updated = await assertContext.Addresses.SingleAsync(TestContext.Current.CancellationToken);
        updated.AddressLine1.Should().Be("Updated Street");
        updated.Postcode.Should().Be("UP1 1DT");
    }

    [Fact]
    public async Task DeleteAddressAsync_ShouldReturnNotFound_WhenAddressMissing()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var sut = new AddressService(context);

        // Act
        var result = await sut.DeleteAddressAsync(999, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task DeleteAddressAsync_ShouldReturnNoContent_WhenAddressExists()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var arrangeContext = CreateContext(dbName);
        SeedAddressGraph(arrangeContext);
        await arrangeContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await using var actContext = CreateContext(dbName);
        var sut = new AddressService(actContext);

        var id = await actContext.Addresses.Select(x => x.Id).SingleAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await sut.DeleteAddressAsync(id, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        await using var assertContext = CreateContext(dbName);
        var count = await assertContext.Addresses.CountAsync(TestContext.Current.CancellationToken);
        count.Should().Be(0);
    }

    private static ApiDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApiDbContext(options);
    }

    private static long SeedAddressGraph(ApiDbContext context)
    {
        var venue = new Venue { Id = 1, Name = "Venue A", CreatedDate = DateTime.UtcNow };
        var connectionType = new ConnectionType { Id = 1, Name = "Password", CreatedDate = DateTime.UtcNow };
        var wifi = new WifiLoginDetails { Id = 1, Ssid = "ssid", EncryptedPassword = "pwd", CreatedDate = DateTime.UtcNow };
        var connectionInfo = new ConnectionInformation
        {
            Id = 1,
            ConnectionTypeId = 1,
            WifiLoginDetailsId = 1,
            CreatedDate = DateTime.UtcNow
        };
        var address = new Address
        {
            Id = 1,
            AddressLine1 = "1 Test Street",
            Postcode = "TE1 1ST",
            VenueId = 1,
            ConnectionInformationId = 1,
            CreatedDate = DateTime.UtcNow,
            Venue = venue
        };

        context.Venues.Add(venue);
        context.ConnectionTypes.Add(connectionType);
        context.WifiLoginDetails.Add(wifi);
        context.ConnectionInformation.Add(connectionInfo);
        context.Addresses.Add(address);

        return address.Id;
    }
}