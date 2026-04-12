using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AwesomeAssertions;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.WifiDetails;
using WifiWarriorAPI.Tests.IntegrationTests.Infrastructure;
using Xunit;

namespace WifiWarriorAPI.Tests.IntegrationTests;

/// <summary>
/// Tests for the Wi-Fi details controller.
/// </summary>
public class WifiDetailsControllerTests : IAsyncLifetime
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
        var response = await _client.GetAsync("/api/wifidetails", TestContext.Current.CancellationToken);

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
        var response = await _client.GetAsync("/api/wifidetails", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var rows = await response.Content.ReadFromJsonAsync<List<WifiDetailResponse>>(TestContext.Current.CancellationToken);
        rows.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenWifiDetailsDoNotExist()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/wifidetails/999", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WithDecryptedPassword_WhenWifiDetailsExist_AndRoleCanEdit()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createRequest = new CreateWifiDetailRequest
        {
            Ssid = $"Guest-{Guid.NewGuid():N}",
            Password = "secret-for-getbyid"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/wifidetails", createRequest, TestContext.Current.CancellationToken);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<WifiDetailResponse>(TestContext.Current.CancellationToken);
        created.Should().NotBeNull();

        // Act
        var getResponse = await _client.GetAsync($"/api/wifidetails/{created!.Id}", TestContext.Current.CancellationToken);

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<WifiDetailResponse>(TestContext.Current.CancellationToken);
        fetched.Should().NotBeNull();
        fetched.Id.Should().Be(created.Id);
        fetched.Ssid.Should().Be(createRequest.Ssid);
        fetched.Password.Should().Be(createRequest.Password);
    }
    
    [Fact]
    public async Task Post_ShouldReturnUnauthorized_WhenNoToken()
    {
        // Arrange
        var request = new CreateWifiDetailRequest
        {
            Ssid = "Guest",
            Password = "secret"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/wifidetails", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_ShouldReturnCreated_WhenRoleCanEdit_AndPayloadValid()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateWifiDetailRequest
        {
            Ssid = $"Guest-{Guid.NewGuid():N}",
            Password = "secret"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/wifidetails", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<WifiDetailResponse>(TestContext.Current.CancellationToken);
        created.Should().NotBeNull();
        created!.Ssid.Should().Be(request.Ssid);
        created.Password.Should().Be(request.Password);
        
        var getResponse = await _client.GetAsync($"/api/wifidetails/{created.Id}", TestContext.Current.CancellationToken);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var fetched = await getResponse.Content.ReadFromJsonAsync<WifiDetailResponse>(TestContext.Current.CancellationToken);
        fetched.Should().NotBeNull();
        fetched.Password.Should().NotBeNull();
        fetched.Password.Should().Be(request.Password);
    }

    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenWifiDetailsDoNotExist_AndRoleCanEdit()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new UpdateWifiDetailRequest
        {
            Ssid = "Updated",
            Password = "updated-secret"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/wifidetails/999", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Put_ShouldReturnNoContent_AndPersistUpdatedPassword_WhenRoleCanEdit()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createRequest = new CreateWifiDetailRequest
        {
            Ssid = $"Guest-{Guid.NewGuid():N}",
            Password = "initial-secret"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/wifidetails", createRequest, TestContext.Current.CancellationToken);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<WifiDetailResponse>(TestContext.Current.CancellationToken);
        created.Should().NotBeNull();

        var updateRequest = new UpdateWifiDetailRequest
        {
            Ssid = "updated-ssid",
            Password = "updated-secret"
        };

        // Act
        var putResponse = await _client.PutAsJsonAsync($"/api/wifidetails/{created.Id}", updateRequest, TestContext.Current.CancellationToken);

        // Assert
        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/wifidetails/{created.Id}", TestContext.Current.CancellationToken);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<WifiDetailResponse>(TestContext.Current.CancellationToken);
        fetched.Should().NotBeNull();
        fetched.Ssid.Should().Be(updateRequest.Ssid);
        fetched.Password.Should().Be(updateRequest.Password);
    }
    
    [Fact]
    public async Task Delete_ShouldReturnForbidden_WhenRoleCannotDelete()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/wifidetails/1", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenWifiDetailsDoNotExist_AndRoleCanDelete()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Administrator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/wifidetails/999", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
