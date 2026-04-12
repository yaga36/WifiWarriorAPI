using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.WifiDetails;
using WifiWarriorAPI.Services;
using Xunit;

namespace WifiWarriorAPI.Tests.UnitTests;

/// <summary>
/// Unit tests for <see cref="WifiDetailsService"/>.
/// </summary>
public class WifiDetailsServiceTests
{
    private readonly Mock<ICredentialsProtector> _mockCredentialsProtector;

    public WifiDetailsServiceTests()
    {
        _mockCredentialsProtector = new Mock<ICredentialsProtector>();
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedRows_WhenRowsExist()
    {
        // Arrange
        const string password1 = "supersecret1";
        const string password2 = "supersecret2";
        var encryptedPassword1 = Guid.NewGuid().ToString();
        var encryptedPassword2 = Guid.NewGuid().ToString();
        
        await using var context = CreateContext(Guid.NewGuid().ToString());
        SeedWifiDetails(context, encryptedPassword1, encryptedPassword2);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        _mockCredentialsProtector.Setup(x => x.Decrypt(encryptedPassword1))
            .Returns(password1);
        _mockCredentialsProtector.Setup(x => x.Decrypt(encryptedPassword2))
            .Returns(password2);
        
        var sut = new WifiDetailsService(context, _mockCredentialsProtector.Object);

        // Act
        var result = await sut.GetAllAsync(TestContext.Current.CancellationToken);

        // Assert
        result.Count.Should().Be(2);
        result.Select(x => x.Ssid).Should().BeEquivalentTo("ssid-1", "ssid-2");
        result.Select(x => x.Password).Should().BeEquivalentTo(password1, password2);
        
        _mockCredentialsProtector.Verify(x => x.Decrypt(encryptedPassword1), Times.Once);
        _mockCredentialsProtector.Verify(x => x.Decrypt(encryptedPassword2), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnRow_WhenExists()
    {
        // Arrange
        const string password = "supersecret1";
        var encryptedPassword = Guid.NewGuid().ToString();
        
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var id = SeedWifiDetails(context, encryptedPassword);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        _mockCredentialsProtector.Setup(x => x.Decrypt(encryptedPassword))
            .Returns(password);
        
        var sut = new WifiDetailsService(context, _mockCredentialsProtector.Object);

        // Act
        var result = await sut.GetByIdAsync(id, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Ssid.Should().Be("ssid-1");
        result.Password.Should().Be(password);
        
        _mockCredentialsProtector.Verify(x => x.Decrypt(encryptedPassword), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenMissing()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var sut = new WifiDetailsService(context, _mockCredentialsProtector.Object);

        // Act
        var result = await sut.GetByIdAsync(999, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreated_WhenValid()
    {
        // Arrange
        const string password = "secret";
        var encryptedPassword = Guid.NewGuid().ToString();
        await using var context = CreateContext(Guid.NewGuid().ToString());
        
        _mockCredentialsProtector.Setup(x => x.Encrypt(password))
            .Returns(encryptedPassword);
        
        var sut = new WifiDetailsService(context, _mockCredentialsProtector.Object);
        var request = new CreateWifiDetailRequest
        {
            Ssid = "Guest",
            Password = password
        };

        // Act
        var result = await sut.CreateAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value.Should().NotBeNull();
        result.Value!.Ssid.Should().Be("Guest");
        result.Value.Password.Should().Be(encryptedPassword);
        
        _mockCredentialsProtector.Verify(x => x.Encrypt(password), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNotFound_WhenMissing()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var sut = new WifiDetailsService(context, _mockCredentialsProtector.Object);
        var request = new UpdateWifiDetailRequest
        {
            Ssid = "Updated",
            Password = "updated-secret"
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
        const string password = "updated-secret";
        var encryptedPassword = Guid.NewGuid().ToString();
        await using var arrangeContext = CreateContext(dbName);
        var id = SeedWifiDetails(arrangeContext);
        await arrangeContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        _mockCredentialsProtector.Setup(x => x.Encrypt(password))
            .Returns(encryptedPassword);
        
        await using var actContext = CreateContext(dbName);
        var sut = new WifiDetailsService(actContext, _mockCredentialsProtector.Object);
        var request = new UpdateWifiDetailRequest
        {
            Ssid = "updated-ssid",
            Password = password
        };

        // Act
        var result = await sut.UpdateAsync(id, request, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        await using var assertContext = CreateContext(dbName);
        var updated = await assertContext.WifiLoginDetails.SingleAsync(x => x.Id == id, TestContext.Current.CancellationToken);
        updated.Ssid.Should().Be("updated-ssid");
        updated.EncryptedPassword.Should().Be(encryptedPassword);
        
        _mockCredentialsProtector.Verify(x => x.Encrypt(password), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenMissing()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var sut = new WifiDetailsService(context, _mockCredentialsProtector.Object);

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
        var id = SeedWifiDetails(arrangeContext);
        await arrangeContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        await using var actContext = CreateContext(dbName);
        var sut = new WifiDetailsService(actContext, _mockCredentialsProtector.Object);

        // Act
        var result = await sut.DeleteAsync(id, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        await using var assertContext = CreateContext(dbName);
        var count = await assertContext.WifiLoginDetails.CountAsync(TestContext.Current.CancellationToken);
        count.Should().Be(1);
    }

    private static ApiDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApiDbContext(options);
    }

    private static long SeedWifiDetails(
        ApiDbContext context,
        string? encryptedPassword1 = null,
        string? encryptedPassword2 = null)
    {
        context.WifiLoginDetails.AddRange(
            new WifiLoginDetails
            {
                Id = 1,
                Ssid = "ssid-1",
                EncryptedPassword = encryptedPassword1 ?? "pwd-1",
                CreatedDate = DateTime.UtcNow
            },
            new WifiLoginDetails
            {
                Id = 2,
                Ssid = "ssid-2",
                EncryptedPassword = encryptedPassword2 ?? "pwd-2",
                CreatedDate = DateTime.UtcNow
            });

        return 1;
    }
}
