using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using AwesomeAssertions;
using Microsoft.IdentityModel.Tokens;
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
        var token = CreateTestToken("Test");
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
        var token = CreateTestToken(nameof(Role.User));
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
        var token = CreateTestToken(nameof(Role.Moderator));
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
        var token = CreateTestToken(nameof(Role.Moderator));
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
        var token = CreateTestToken(nameof(Role.Administrator));
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
        var token = CreateTestToken(nameof(Role.Administrator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Act
        var response = await _client.DeleteAsync($"/api/venue/{id}", TestContext.Current.CancellationToken);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private static HttpClient CreateClient()
    {
        var factory = new CustomWebApplicationFactory();
        return factory.CreateClient();
    }
    
    private static string CreateTestToken(string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "test-user-id"),
            new Claim(ClaimTypes.Name, "test-user"),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestConstants.JwtSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: TestConstants.JwtIssuer,
            audience: TestConstants.JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}