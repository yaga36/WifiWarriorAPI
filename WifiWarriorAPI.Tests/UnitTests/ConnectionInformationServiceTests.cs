using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.ConnectionInformations;
using WifiWarriorAPI.Services;
using Xunit;

namespace WifiWarriorAPI.Tests.UnitTests;

/// <summary>
/// Unit tests for <see cref="ConnectionInformationService"/>.
/// </summary>
public class ConnectionInformationServiceTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedRows_WhenExist()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        SeedConnectionInformation(context);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new ConnectionInformationService(context);

        // Act
        var result = await sut.GetAllAsync(TestContext.Current.CancellationToken);

        // Assert
        result.Count.Should().Be(1);
        result.First().ConnectionTypeName.Should().Be("Password");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnRow_WhenExists()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var id = SeedConnectionInformation(context);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new ConnectionInformationService(context);

        // Act
        var result = await sut.GetByIdAsync(id, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenMissing()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var sut = new ConnectionInformationService(context);

        // Act
        var result = await sut.GetByIdAsync(999, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreated_WhenValid()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var sut = new ConnectionInformationService(context);

        // Act
        var result = await sut.CreateAsync(
            new CreateConnectionInformationRequest { ConnectionTypeId = 1, WifiLoginDetailsId = 1 },
            TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNotFound_WhenMissing()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var sut = new ConnectionInformationService(context);

        // Act
        var result = await sut.UpdateAsync(
            999,
            new UpdateConnectionInformationRequest { ConnectionTypeId = 1, WifiLoginDetailsId = 1 },
            TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNoContent_WhenExists()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var arrange = CreateContext(dbName);
        var id = SeedConnectionInformation(arrange);
        await arrange.SaveChangesAsync(TestContext.Current.CancellationToken);

        await using var act = CreateContext(dbName);
        var sut = new ConnectionInformationService(act);

        // Act
        var result = await sut.UpdateAsync(
            id,
            new UpdateConnectionInformationRequest { ConnectionTypeId = 2, WifiLoginDetailsId = 2 },
            TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenMissing()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var sut = new ConnectionInformationService(context);

        // Act
        var result = await sut.DeleteAsync(999, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNoContent_WhenExists()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var arrange = CreateContext(dbName);
        var id = SeedConnectionInformation(arrange);
        await arrange.SaveChangesAsync(TestContext.Current.CancellationToken);

        await using var act = CreateContext(dbName);
        var sut = new ConnectionInformationService(act);

        // Act
        var result = await sut.DeleteAsync(id, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    private static ApiDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new ApiDbContext(options);
    }

    private static long SeedConnectionInformation(ApiDbContext context)
    {
        context.ConnectionTypes.AddRange(
            new ConnectionType { Id = 1, Name = "Password", CreatedDate = DateTime.UtcNow },
            new ConnectionType { Id = 2, Name = "Login", CreatedDate = DateTime.UtcNow });

        context.WifiLoginDetails.AddRange(
            new WifiLoginDetails { Id = 1, Ssid = "ssid-1", Password = "pwd-1", CreatedDate = DateTime.UtcNow },
            new WifiLoginDetails { Id = 2, Ssid = "ssid-2", Password = "pwd-2", CreatedDate = DateTime.UtcNow });

        var row = new ConnectionInformation
        {
            Id = 1,
            ConnectionTypeId = 1,
            WifiLoginDetailsId = 1,
            CreatedDate = DateTime.UtcNow
        };

        context.ConnectionInformation.Add(row);
        return row.Id;
    }
}