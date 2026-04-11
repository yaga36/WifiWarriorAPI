using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Users;
using WifiWarriorAPI.Services;
using WifiWarriorAPI.Tests.IntegrationTests.Infrastructure;
using Xunit;

namespace WifiWarriorAPI.Tests.UnitTests;

/// <summary>
/// Unit tests for <see cref="UserService"/>.
/// </summary>
public class UserServiceTests
{
    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnMappedUsers_WhenUsersExist()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        context.Users.AddRange(
            new Users { FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", UserName = "jane@example.com" },
            new Users { FirstName = "John", LastName = "Doe", Email = "john@example.com", UserName = "john@example.com" });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var userManager = TestHelpers.CreateUserManagerMock();
        var logger = new Mock<ILogger<UserService>>();
        var sut = new UserService(context, userManager.Object, logger.Object);

        // Act
        var result = await sut.GetAllUsersAsync(TestContext.Current.CancellationToken);

        // Assert
        result.Count.Should().Be(2);
        result.Select(x => x.Email).Should().BeEquivalentTo(["jane@example.com", "john@example.com"]);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenExists()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var user = new Users { FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", UserName = "jane@example.com" };
        context.Users.Add(user);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var userManager = TestHelpers.CreateUserManagerMock();
        var logger = new Mock<ILogger<UserService>>();
        var sut = new UserService(context, userManager.Object, logger.Object);

        // Act
        var result = await sut.GetUserByIdAsync(user.Id, TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("jane@example.com");
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var userManager = TestHelpers.CreateUserManagerMock();
        var logger = new Mock<ILogger<UserService>>();
        var sut = new UserService(context, userManager.Object, logger.Object);

        // Act
        var result = await sut.GetUserByIdAsync("missing-id", TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnCreated_WhenCreateSucceeds()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var userManager = TestHelpers.CreateUserManagerMock();
        var logger = new Mock<ILogger<UserService>>();

        userManager
            .Setup(x => x.CreateAsync(It.IsAny<Users>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var sut = new UserService(context, userManager.Object, logger.Object);

        var request = new CreateUserRequest
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@example.com",
            Password = "P@ssw0rd123!",
            PhoneNumber = "07123456789"
        };

        // Act
        var result = await sut.CreateUserAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Value.Should().NotBeNull();
        result.Value!.Email.Should().Be("jane@example.com");
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnConflict_WhenDuplicateEmail()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var userManager = TestHelpers.CreateUserManagerMock();
        var logger = new Mock<ILogger<UserService>>();

        userManager
            .Setup(x => x.CreateAsync(It.IsAny<Users>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "DuplicateEmail", Description = "Email already exists." }));

        var sut = new UserService(context, userManager.Object, logger.Object);

        var request = new CreateUserRequest
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@example.com",
            Password = "P@ssw0rd123!"
        };

        // Act
        var result = await sut.CreateUserAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var userManager = TestHelpers.CreateUserManagerMock();
        var logger = new Mock<ILogger<UserService>>();

        userManager
            .Setup(x => x.CreateAsync(It.IsAny<Users>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "PasswordTooShort", Description = "Password too short." }));

        var sut = new UserService(context, userManager.Object, logger.Object);

        // Act
        var result = await sut.CreateUserAsync(new CreateUserRequest
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane@example.com",
            Password = "x"
        }, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldReturnNotFound_WhenUserMissing()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var userManager = TestHelpers.CreateUserManagerMock();
        var logger = new Mock<ILogger<UserService>>();
        var sut = new UserService(context, userManager.Object, logger.Object);

        // Act
        var result = await sut.UpdateUserAsync("missing-id", new UpdateUserRequest
        {
            FirstName = "Updated",
            LastName = "User",
            Email = "updated@example.com"
        }, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldReturnNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var user = new Users { FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", UserName = "jane@example.com" };
        context.Users.Add(user);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var userManager = TestHelpers.CreateUserManagerMock();
        var logger = new Mock<ILogger<UserService>>();

        userManager
            .Setup(x => x.UpdateAsync(It.IsAny<Users>()))
            .ReturnsAsync(IdentityResult.Success);

        var sut = new UserService(context, userManager.Object, logger.Object);

        // Act
        var result = await sut.UpdateUserAsync(user.Id, new UpdateUserRequest
        {
            FirstName = "Updated",
            LastName = "User",
            Email = "updated@example.com",
            PhoneNumber = "07000000000"
        }, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        userManager.Verify(x => x.UpdateAsync(
                It.Is<Users>(u =>
                    u.Id == user.Id &&
                    u.FirstName == "Updated" &&
                    u.LastName == "User" &&
                    u.Email == "updated@example.com" &&
                    u.UserName == "updated@example.com" &&
                    u.NormalizedEmail == "UPDATED@EXAMPLE.COM" &&
                    u.NormalizedUserName == "UPDATED@EXAMPLE.COM")),
            Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldReturnBadRequest_WhenUpdateFails()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var user = new Users { FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", UserName = "jane@example.com" };
        context.Users.Add(user);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var userManager = TestHelpers.CreateUserManagerMock();
        var logger = new Mock<ILogger<UserService>>();

        userManager
            .Setup(x => x.UpdateAsync(It.IsAny<Users>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed." }));

        var sut = new UserService(context, userManager.Object, logger.Object);

        // Act
        var result = await sut.UpdateUserAsync(user.Id, new UpdateUserRequest
        {
            FirstName = "Updated",
            LastName = "User",
            Email = "updated@example.com"
        }, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldReturnNotFound_WhenUserMissing()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var userManager = TestHelpers.CreateUserManagerMock();
        var logger = new Mock<ILogger<UserService>>();
        var sut = new UserService(context, userManager.Object, logger.Object);

        // Act
        var result = await sut.DeleteUserAsync("missing-id", TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldReturnNoContent_WhenDeleteSucceeds()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var user = new Users { FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", UserName = "jane@example.com" };
        context.Users.Add(user);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var userManager = TestHelpers.CreateUserManagerMock();
        var logger = new Mock<ILogger<UserService>>();

        userManager
            .Setup(x => x.DeleteAsync(It.IsAny<Users>()))
            .ReturnsAsync(IdentityResult.Success);

        var sut = new UserService(context, userManager.Object, logger.Object);

        // Act
        var result = await sut.DeleteUserAsync(user.Id, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        userManager.Verify(x => x.DeleteAsync(It.Is<Users>(u => u.Id == user.Id)), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldReturnBadRequest_WhenDeleteFails()
    {
        // Arrange
        await using var context = CreateContext(Guid.NewGuid().ToString());
        var user = new Users { FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", UserName = "jane@example.com" };
        context.Users.Add(user);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var userManager = TestHelpers.CreateUserManagerMock();
        var logger = new Mock<ILogger<UserService>>();

        userManager
            .Setup(x => x.DeleteAsync(It.IsAny<Users>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Delete failed." }));

        var sut = new UserService(context, userManager.Object, logger.Object);

        // Act
        var result = await sut.DeleteUserAsync(user.Id, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    private static ApiDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApiDbContext(options);
    }
}