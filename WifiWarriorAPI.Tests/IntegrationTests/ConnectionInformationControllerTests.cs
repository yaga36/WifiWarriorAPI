using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AwesomeAssertions;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.ConnectionInformations;
using WifiWarriorAPI.Tests.IntegrationTests.Infrastructure;
using Xunit;

namespace WifiWarriorAPI.Tests.IntegrationTests;

/// <summary>
/// Tests for the connection information controller.
/// </summary>
public class ConnectionInformationControllerTests : IAsyncLifetime
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
        var response = await _client.GetAsync("/api/connectioninformation", TestContext.Current.CancellationToken);
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
        var response = await _client.GetAsync("/api/connectioninformation", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenMissing()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/connectioninformation/999999", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturnUnauthorized_WhenNoToken()
    {
        // Arrange
        var request = new CreateConnectionInformationRequest { ConnectionTypeId = 99, WifiLoginDetailsId = 99 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/connectioninformation", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_ShouldReturnCreated_WhenRoleCanEdit()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateConnectionInformationRequest { ConnectionTypeId = 99, WifiLoginDetailsId = 99 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/connectioninformation", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<ConnectionInformationResponse>(TestContext.Current.CancellationToken);
        created.Should().NotBeNull();
    }

    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenMissing_AndRoleCanEdit()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new UpdateConnectionInformationRequest { ConnectionTypeId = 1, WifiLoginDetailsId = 1 };

        // Act
        var response = await _client.PutAsJsonAsync("/api/connectioninformation/999999", request, TestContext.Current.CancellationToken);

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
        var response = await _client.DeleteAsync("/api/connectioninformation/1", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenMissing_AndRoleCanDelete()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Administrator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/connectioninformation/999999", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}