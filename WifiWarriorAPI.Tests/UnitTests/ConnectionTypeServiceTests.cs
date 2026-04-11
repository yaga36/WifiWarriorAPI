using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.ConnectionTypes;
using WifiWarriorAPI.Services;
using Xunit;

namespace WifiWarriorAPI.Tests.UnitTests;

/// <summary>
/// Unit tests for <see cref="ConnectionTypeService"/>.
/// </summary>
public class ConnectionTypeServiceTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedRows_WhenRowsExist()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        SeedConnectionTypes(context);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new ConnectionTypeService(context);

        // Act
        var result = await sut.GetAllAsync(TestContext.Current.CancellationToken);

        // Assert
        result.Count.Should().Be(2);
        result.Select(x => x.Name).Should().BeEquivalentTo(["Password", "Login"]);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnRow_WhenExists()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var id = SeedConnectionTypes(context);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new ConnectionTypeService(context);

        // Act
        var result = await sut.GetByIdAsync(id, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Name.Should().Be("Password");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenMissing()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var sut = new ConnectionTypeService(context);

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
        var sut = new ConnectionTypeService(context);
        var request = new CreateConnectionTypeRequest
        {
            Name = "Enterprise"
        };

        // Act
        var result = await sut.CreateAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Enterprise");
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNotFound_WhenMissing()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var sut = new ConnectionTypeService(context);
        var request = new UpdateConnectionTypeRequest
        {
            Name = "Updated"
        };

        // Act
        var result = await sut.UpdateAsync(999, request, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNoContent_WhenRowExists()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var arrangeContext = CreateContext(dbName);
        var id = SeedConnectionTypes(arrangeContext);
        await arrangeContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await using var actContext = CreateContext(dbName);
        var sut = new ConnectionTypeService(actContext);
        var request = new UpdateConnectionTypeRequest
        {
            Name = "Updated"
        };

        // Act
        var result = await sut.UpdateAsync(id, request, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        await using var assertContext = CreateContext(dbName);
        var updated = await assertContext.ConnectionTypes.SingleAsync(x => x.Id == id, TestContext.Current.CancellationToken);
        updated.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenMissing()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var sut = new ConnectionTypeService(context);

        // Act
        var result = await sut.DeleteAsync(999, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNoContent_WhenRowExists()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var arrangeContext = CreateContext(dbName);
        var id = SeedConnectionTypes(arrangeContext);
        await arrangeContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await using var actContext = CreateContext(dbName);
        var sut = new ConnectionTypeService(actContext);

        // Act
        var result = await sut.DeleteAsync(id, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        await using var assertContext = CreateContext(dbName);
        var count = await assertContext.ConnectionTypes.CountAsync(TestContext.Current.CancellationToken);
        count.Should().Be(1);
    }

    private static ApiDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApiDbContext(options);
    }

    private static long SeedConnectionTypes(ApiDbContext context)
    {
        context.ConnectionTypes.AddRange(
            new ConnectionType
            {
                Id = 1,
                Name = "Password",
                CreatedDate = DateTime.UtcNow
            },
            new ConnectionType
            {
                Id = 2,
                Name = "Login",
                CreatedDate = DateTime.UtcNow
            });

        return 1;
    }
}
