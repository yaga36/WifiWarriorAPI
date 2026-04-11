using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AwesomeAssertions;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Addresses;
using WifiWarriorAPI.Tests.IntegrationTests.Infrastructure;
using Xunit;

namespace WifiWarriorAPI.Tests.IntegrationTests;

/// <summary>
/// Tests for the address controller.
/// </summary>
public class AddressControllerTests : IAsyncLifetime
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
        var response = await _client.GetAsync("/api/address", TestContext.Current.CancellationToken);

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
        var response = await _client.GetAsync("/api/address", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var addresses = await response.Content.ReadFromJsonAsync<List<AddressResponse>>(TestContext.Current.CancellationToken);
        addresses.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenAddressDoesNotExist()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/address/999", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturnUnauthorized_WhenNoToken()
    {
        // Arrange
        var request = new CreateAddressRequest
        {
            AddressLine1 = "1 Test Street",
            Postcode = "TE1 1ST",
            VenueId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/address", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_ShouldReturnCreated_WhenRoleCanEdit_AndPayloadValid()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateAddressRequest
        {
            AddressLine1 = "1 Test Street",
            AddressLine2 = "Suite 2",
            Area = "Test Area",
            County = "Test County",
            Postcode = "TE1 1ST",
            Latitude = 51.5,
            Longitude = -0.12,
            VenueId = 1,
            ConnectionInformationId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/address", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<AddressResponse>(TestContext.Current.CancellationToken);
        created.Should().NotBeNull();
        created.AddressLine1.Should().Be("1 Test Street");
        created.VenueId.Should().Be(1);
    }

    [Fact]
    public async Task Put_ShouldReturnNoContent_WhenAddressExists_AndRoleCanEdit()
    {
        // Arrange
        var moderatorToken = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", moderatorToken);

        var createRequest = new CreateAddressRequest
        {
            AddressLine1 = "10 Original Street",
            Postcode = "OR1 1AA",
            VenueId = 1,
            ConnectionInformationId = 1
        };

        var createResponse = await _client.PostAsJsonAsync("/api/address", createRequest, TestContext.Current.CancellationToken);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<AddressResponse>(TestContext.Current.CancellationToken);
        created.Should().NotBeNull();

        var updateRequest = new UpdateAddressRequest
        {
            AddressLine1 = "10 Updated Street",
            AddressLine2 = "Suite 4",
            Area = "Updated Area",
            County = "Updated County",
            Postcode = "UP1 1DT",
            Latitude = 51.52,
            Longitude = -0.11,
            VenueId = 1,
            ConnectionInformationId = 1
        };

        // Act
        var updateResponse = await _client.PutAsJsonAsync($"/api/address/{created!.Id}", updateRequest, TestContext.Current.CancellationToken);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenAddressDoesNotExist_AndRoleCanEdit()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new UpdateAddressRequest
        {
            AddressLine1 = "Updated Line",
            Postcode = "UP1 1DT",
            VenueId = 1,
            ConnectionInformationId = 1
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/address/999", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenAddressExists_AndRoleCanDelete()
    {
        // Arrange
        var moderatorToken = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", moderatorToken);

        var createRequest = new CreateAddressRequest
        {
            AddressLine1 = "20 Delete Street",
            Postcode = "DL1 1TE",
            VenueId = 1,
            ConnectionInformationId = 1
        };

        var createResponse = await _client.PostAsJsonAsync("/api/address", createRequest, TestContext.Current.CancellationToken);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<AddressResponse>(TestContext.Current.CancellationToken);
        created.Should().NotBeNull();

        var adminToken = TestHelpers.CreateTestToken(nameof(Role.Administrator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/address/{created!.Id}", TestContext.Current.CancellationToken);

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task Delete_ShouldReturnForbidden_WhenRoleCannotDelete()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/address/1", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenAddressDoesNotExist_AndRoleCanDelete()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Administrator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/address/999", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}