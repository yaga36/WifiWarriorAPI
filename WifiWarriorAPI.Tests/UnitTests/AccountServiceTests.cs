using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Accounts;
using WifiWarriorAPI.Services;
using WifiWarriorAPI.Tests.IntegrationTests.Infrastructure;
using Xunit;

namespace WifiWarriorAPI.Tests.UnitTests;

public class AccountServiceTests
{
    
    [Fact]
    public async Task RegisterAsync_ShouldReturnAccepted_WhenUserCreatedAndRoleAssigned()
    {
        // Arrange
        var userManager = TestHelpers.CreateUserManagerMock();
        var authManager = new Mock<IAuthManager>();

        userManager
            .Setup(x => x.CreateAsync(It.IsAny<Users>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        userManager
            .Setup(x => x.AddToRolesAsync(
                It.IsAny<Users>(),
                It.Is<IEnumerable<string>>(roles => roles.Contains(nameof(Role.User)))))
            .ReturnsAsync(IdentityResult.Success);

        var sut = new AccountService(userManager.Object, authManager.Object);

        var request = new RegisterAccountRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "P@ssw0rd123!",
            PhoneNumber = "07123456789"
        };

        // Act
        var result = await sut.RegisterAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status202Accepted);

        userManager.Verify(
            x => x.CreateAsync(
                It.Is<Users>(u =>
                    u.Email == request.Email &&
                    u.UserName == request.Email &&
                    u.FirstName == request.FirstName &&
                    u.LastName == request.LastName),
                request.Password),
            Times.Once);

        userManager.Verify(
            x => x.AddToRolesAsync(
                It.IsAny<Users>(),
                It.Is<IEnumerable<string>>(roles => roles.Contains(nameof(Role.User)))),
            Times.Once);
    }
    
    [Fact]
    public async Task RegisterAsync_ShouldReturnBadRequest_WhenRoleAssignmentFails()
    {
        // Arrange
        var userManager = TestHelpers.CreateUserManagerMock();
        var authManager = new Mock<IAuthManager>();

        userManager
            .Setup(x => x.CreateAsync(It.IsAny<Users>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        userManager
            .Setup(x => x.AddToRolesAsync(
                It.IsAny<Users>(),
                It.Is<IEnumerable<string>>(roles => roles.Contains(nameof(Role.User)))))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Invalid role" }));

        var sut = new AccountService(userManager.Object, authManager.Object);

        var request = new RegisterAccountRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "P@ssw0rd123!",
            PhoneNumber = "07123456789"
        };

        // Act
        var result = await sut.RegisterAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
    
    [Fact]
    public async Task RegisterAsync_ShouldReturnBadRequest_WhenCreateUserFails()
    {
        var userManager = TestHelpers.CreateUserManagerMock();
        var authManager = new Mock<IAuthManager>();

        userManager
            .Setup(x => x.CreateAsync(It.IsAny<Users>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Duplicate email" }));

        var sut = new AccountService(userManager.Object, authManager.Object);

        var result = await sut.RegisterAsync(new RegisterAccountRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "P@ssw0rd123!"
        }, TestContext.Current.CancellationToken);

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        userManager.Verify(
            x => x.AddToRolesAsync(
                It.IsAny<Users>(),
                It.IsAny<IEnumerable<string>>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrow_WhenDependencyThrows()
    {
        var userManager = TestHelpers.CreateUserManagerMock();
        var authManager = new Mock<IAuthManager>();

        userManager
            .Setup(x => x.CreateAsync(It.IsAny<Users>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Something went terribly wrong"));

        var sut = new AccountService(userManager.Object, authManager.Object);

        var act = () => sut.RegisterAsync(new RegisterAccountRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "P@ssw0rd123!"
        }, TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Something went terribly wrong");
        
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnAcceptedWithToken_WhenCredentialsValid()
    {
        var userManager = TestHelpers.CreateUserManagerMock();
        var authManager = new Mock<IAuthManager>();

        var user = new Users
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };
        
        authManager
            .Setup(x => x.ValidateUser(It.IsAny<LoginInfo>()))
            .ReturnsAsync(user);

        authManager
            .Setup(x => x.CreateToken(It.IsAny<Users>()))
            .ReturnsAsync("test-jwt-token");
        
        var sut = new AccountService(userManager.Object, authManager.Object);

        var result = await sut.LoginAsync(
            new LoginAccountRequest { Email = "john.doe@example.com", Password = "P@ssword1!" },
            TestContext.Current.CancellationToken);

        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status202Accepted);
        result.Value.Should().NotBeNull();
        result.Value.AccessToken.Should().Be("test-jwt-token");
    }
    
    [Fact]
    public async Task LoginAsync_ShouldReturnUnauthorized_WhenCredentialsInvalid()
    {
        var userManager = TestHelpers.CreateUserManagerMock();
        var authManager = new Mock<IAuthManager>();

        authManager
            .Setup(x => x.ValidateUser(It.IsAny<LoginInfo>()))
            .ReturnsAsync((Users?)null);

        var sut = new AccountService(userManager.Object, authManager.Object);

        var result = await sut.LoginAsync(
            new LoginAccountRequest { Email = "a@b.com", Password = "bad" },
            TestContext.Current.CancellationToken);

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }
    
    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenDependencyThrows()
    {
        var userManager = TestHelpers.CreateUserManagerMock();
        var authManager = new Mock<IAuthManager>();

        authManager
            .Setup(x => x.ValidateUser(It.IsAny<LoginInfo>()))
            .ThrowsAsync(new Exception("Something went terribly wrong"));

        var sut = new AccountService(userManager.Object, authManager.Object);

        var act = () => sut.LoginAsync(
            new LoginAccountRequest { Email = "a@b.com", Password = "bad" },
            TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<Exception>().WithMessage("Something went terribly wrong");
    }
}