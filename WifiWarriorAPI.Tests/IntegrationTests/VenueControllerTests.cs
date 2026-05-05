using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Venues;
using WifiWarriorAPI.Tests.IntegrationTests.Infrastructure;
using Xunit;

namespace WifiWarriorAPI.Tests.IntegrationTests;

/// <summary>
/// Tests for the venue controller.
/// </summary>
public class VenueControllerTests : IAsyncLifetime
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
    public async Task Get_ShouldReturnOk_ForAnonymousUser()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/venue", TestContext.Current.CancellationToken);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_ShouldReturnOk_WithCorrectSeededValues()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/venue", TestContext.Current.CancellationToken);
        
        // Assert
        var venues = await response.Content.ReadFromJsonAsync<List<VenueResponse>>(TestContext.Current.CancellationToken);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        venues.Should().NotBeNull();
        venues.Count.Should().Be(1);
        venues.First().Name.Should().Be("Seed Venue");
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenVenueExists()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/venue/1", TestContext.Current.CancellationToken);
        
        // Assert
        var venue = await response.Content.ReadFromJsonAsync<VenueResponse>(TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        venue.Should().NotBeNull();
        venue.Name.Should().Be("Seed Venue");
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenVenueDoesNotExist()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/venue/999", TestContext.Current.CancellationToken);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetFullById_ShouldReturnOk_WhenAggregateExists()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/venue/full/1", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var venueSetup = await response.Content.ReadFromJsonAsync<VenueSetupResponse>(TestContext.Current.CancellationToken);
        venueSetup.Should().NotBeNull();
        venueSetup!.Venue.Name.Should().Be("Seed Venue");
        venueSetup.Address.AddressLine1.Should().Be("Seed Address");
        venueSetup.Connection.ConnectionTypeName.Should().Be("Open");
        venueSetup.WifiDetails.Should().BeNull();
    }

    [Fact]
    public async Task GetFull_ShouldReturnAllAggregates_WithoutNPlusOneCalls()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/venue/full", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var venueSetups = await response.Content.ReadFromJsonAsync<List<VenueSetupResponse>>(TestContext.Current.CancellationToken);
        venueSetups.Should().NotBeNull();
        venueSetups.Should().ContainSingle();
        venueSetups!.Single().Venue.Name.Should().Be("Seed Venue");
        venueSetups.Single().Address.AddressLine1.Should().Be("Seed Address");
        venueSetups.Single().Connection.ConnectionTypeName.Should().Be("Open");
    }

    [Fact]
    public async Task GetFullById_ShouldReturnNotFound_WhenAggregateDoesNotExist()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/venue/full/999", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_ShouldReturnUnauthorised_WhenNoToken()
    {
        // Arrange
        var request = new CreateVenueRequest
        {
            Name = "New Venue",
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/venue", request, TestContext.Current.CancellationToken);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_ShouldReturnForbidden_WhenRoleCannotSubmit()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken("Test");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request = new CreateVenueRequest
        {
            Name = "New Venue",
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/venue", request, TestContext.Current.CancellationToken);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Post_ShouldReturnCreated_WhenRoleCanSubmit()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.User));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request = new CreateVenueRequest
        {
            Name = "New Venue",
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/venue", request, TestContext.Current.CancellationToken);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task PostFull_ShouldReturnCreated_AndPersistAggregate_WhenRoleCanSubmit()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.User));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateVenueSetupRequest
        {
            VenueName = "Onboarded Venue",
            Address = new CreateVenueSetupAddressRequest
            {
                AddressLine1 = "10 Main Road",
                Area = "Town Centre",
                Postcode = "AB12CD"
            },
            Connection = new CreateVenueSetupConnectionRequest
            {
                ConnectionTypeId = 2,
                Ssid = "GuestWifi",
                Password = "supersecret"
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/venue/full", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var payload = await response.Content.ReadFromJsonAsync<VenueSetupResponse>(TestContext.Current.CancellationToken);
        payload.Should().NotBeNull();
        payload!.Venue.Name.Should().Be("Onboarded Venue");
        payload.Address.AddressLine1.Should().Be("10 Main Road");
        payload.Connection.ConnectionTypeId.Should().Be(2);
        payload.WifiDetails.Should().NotBeNull();
        payload.WifiDetails!.Ssid.Should().Be("GuestWifi");
        payload.WifiDetails.Password.Should().Be("supersecret");

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
        db.Venues.Should().ContainSingle(x => x.Name == "Onboarded Venue");
        db.ConnectionInformation.Should().ContainSingle(x => x.ConnectionTypeId == 2 && x.WifiLoginDetailsId != null);
        db.Addresses.Should().ContainSingle(x => x.AddressLine1 == "10 Main Road");
        db.WifiLoginDetails.Should().ContainSingle(x => x.Ssid == "GuestWifi" && x.EncryptedPassword != "supersecret");
    }

    [Fact]
    public async Task PostFull_ShouldReturnBadRequest_WhenProtectedConnectionMissingPassword()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.User));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateVenueSetupRequest
        {
            VenueName = "Onboarded Venue",
            Address = new CreateVenueSetupAddressRequest
            {
                AddressLine1 = "10 Main Road",
                Postcode = "AB12CD"
            },
            Connection = new CreateVenueSetupConnectionRequest
            {
                ConnectionTypeId = 2,
                Ssid = "GuestWifi"
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/venue/full", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
        db.Venues.Should().NotContain(x => x.Name == "Onboarded Venue");
        db.ConnectionInformation.Should().NotContain(x => x.WifiLoginDetailsId != null && x.Id > 1);
    }

    [Fact]
    public async Task Put_ShouldReturnUnauthorized_WhenNoToken()
    {
        // Arrange
        const long id = 1;
        var request = new UpdateVenueRequest
        {
            Name = "Updated Venue",
        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"/api/venue/{id}", request, TestContext.Current.CancellationToken);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenVenueDoesNotExist_AndRoleCanEdit()
    {
        // Arrange
        const long id = 2;
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request = new UpdateVenueRequest
        {
            Name = "Updated Venue",
        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"/api/venue/{id}", request, TestContext.Current.CancellationToken);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Put_ShouldReturnNoContent_WhenVenueExists_AndRoleCanEdit()
    {
        // Arrange
        const long id = 1;
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var request = new UpdateVenueRequest
        {
            Name = "Updated Venue",
        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"/api/venue/{id}", request, TestContext.Current.CancellationToken);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task Delete_ShouldReturnUnauthorized_WhenNoToken()
    {
        // Arrange
        const long id = 1;
        
        // Act
        var response = await _client.DeleteAsync($"/api/venue/{id}", TestContext.Current.CancellationToken);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenVenueDoesNotExist_AndRoleCanDelete()
    {
        // Arrange
        const long id = 2;
        var token = TestHelpers.CreateTestToken(nameof(Role.Administrator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Act
        var response = await _client.DeleteAsync($"/api/venue/{id}", TestContext.Current.CancellationToken);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenVenueExists_AndRoleCanDelete()
    {
        // Arrange
        const long id = 1;
        var token = TestHelpers.CreateTestToken(nameof(Role.Administrator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Act
        var response = await _client.DeleteAsync($"/api/venue/{id}", TestContext.Current.CancellationToken);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}