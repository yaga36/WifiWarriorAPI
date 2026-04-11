using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AwesomeAssertions;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.ConnectionTypes;
using WifiWarriorAPI.Tests.IntegrationTests.Infrastructure;
using Xunit;

namespace WifiWarriorAPI.Tests.IntegrationTests;

/// <summary>
/// Tests for the connection types controller.
/// </summary>
public class ConnectionTypesControllerTests : IAsyncLifetime
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
        var response = await _client.GetAsync("/api/connectiontypes", TestContext.Current.CancellationToken);

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
        var response = await _client.GetAsync("/api/connectiontypes", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var rows = await response.Content.ReadFromJsonAsync<List<ConnectionTypeResponse>>(TestContext.Current.CancellationToken);
        rows.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenConnectionTypeDoesNotExist()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/connectiontypes/999", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturnUnauthorized_WhenNoToken()
    {
        // Arrange
        var request = new CreateConnectionTypeRequest
        {
            Name = "Enterprise"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/connectiontypes", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_ShouldReturnCreated_WhenRoleCanEdit_AndPayloadValid()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateConnectionTypeRequest
        {
            Name = $"Enterprise-{Guid.NewGuid():N}"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/connectiontypes", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<ConnectionTypeResponse>(TestContext.Current.CancellationToken);
        created.Should().NotBeNull();
        created!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenConnectionTypeDoesNotExist_AndRoleCanEdit()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new UpdateConnectionTypeRequest
        {
            Name = "Updated"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/connectiontypes/999", request, TestContext.Current.CancellationToken);

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
        var response = await _client.DeleteAsync("/api/connectiontypes/1", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenConnectionTypeDoesNotExist_AndRoleCanDelete()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Administrator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/connectiontypes/999", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
