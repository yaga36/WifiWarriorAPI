using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AwesomeAssertions;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Users;
using WifiWarriorAPI.Tests.IntegrationTests.Infrastructure;
using Xunit;

namespace WifiWarriorAPI.Tests.IntegrationTests;

/// <summary>
/// Tests for the users controller.
/// </summary>
public class UsersControllerTests : IAsyncLifetime
{
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    public ValueTask InitializeAsync()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task Get_ShouldReturnUnauthorized_WhenNoToken()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/users", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_ShouldReturnOk_WhenRoleCanEdit()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>(TestContext.Current.CancellationToken);
        users.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users/not-found-id", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturnUnauthorized_WhenNoToken()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = $"jane.{Guid.NewGuid():N}@example.com",
            Password = "P@ssw0rd123!",
            PhoneNumber = "07123456789"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_ShouldReturnCreated_WhenRoleCanEdit_AndPayloadValid()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateUserRequest
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = $"jane.{Guid.NewGuid():N}@example.com",
            Password = "P@ssw0rd123!",
            PhoneNumber = "07123456789"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<UserResponse>(TestContext.Current.CancellationToken);
        created.Should().NotBeNull();
        created.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task Post_ShouldReturnConflict_WhenDuplicateEmailProvided()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var email = $"dupe.{Guid.NewGuid():N}@example.com";
        var first = new CreateUserRequest
        {
            FirstName = "First",
            LastName = "User",
            Email = email,
            Password = "P@ssw0rd123!"
        };
        var second = new CreateUserRequest
        {
            FirstName = "Second",
            LastName = "User",
            Email = email,
            Password = "P@ssw0rd123!"
        };

        // Act
        var firstResponse = await _client.PostAsJsonAsync("/api/users", first, TestContext.Current.CancellationToken);
        var secondResponse = await _client.PostAsJsonAsync("/api/users", second, TestContext.Current.CancellationToken);

        // Assert
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenUserDoesNotExist_AndRoleCanEdit()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new UpdateUserRequest
        {
            FirstName = "Updated",
            LastName = "User",
            Email = $"updated.{Guid.NewGuid():N}@example.com",
            PhoneNumber = "07000000000"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/users/not-found-id", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturnForbidden_WhenRoleCannotDelete()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/users/any-id", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}