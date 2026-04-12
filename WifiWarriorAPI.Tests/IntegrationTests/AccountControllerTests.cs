using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WifiWarriorAPI.Data;
using WifiWarriorAPI.Models;
using WifiWarriorAPI.Models.Dtos.Accounts;
using WifiWarriorAPI.Tests.IntegrationTests.Infrastructure;
using Xunit;

namespace WifiWarriorAPI.Tests.IntegrationTests;

public class AccountControllerTests : IAsyncLifetime
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
    public async Task Register_ShouldReturnAccepted_WhenPayloadIsValid()
    {
        // Arrange
        var request = new RegisterAccountRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = $"john.{Guid.NewGuid():N}@example.com",
            Password = "P@ssw0rd123!",
            PhoneNumber = "07123456789"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/account/register",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
    
    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenPayloadIsInvalid()
    {
        // Arrange
        const string invalidJson = """
                                   {
                                     "firstName": "Reece",
                                     "lastName": "Tester",
                                     "password": "P@ssw0rd123!"
                                   }
                                   """;

        using var content = new StringContent(invalidJson, System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(
            "/api/account/register",
            content,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Login_ShouldReturnAccepted_WithToken_WhenCredentialsAreValid()
    {
        // Arrange
        var email = $"john.{Guid.NewGuid():N}@example.com";
        const string password = "P@ssw0rd123!";
        
        var register = new RegisterAccountRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = email,
            Password = password,
        };

        // Act
        var registerResponse = await _client.PostAsJsonAsync(
            "/api/account/register",
            register,
            TestContext.Current.CancellationToken);
        
        var login = new LoginAccountRequest
        {
            Email = email,
            Password = password,
        };
        
        var loginResponse = await _client.PostAsJsonAsync("/api/account/login", login,  TestContext.Current.CancellationToken);
        
        // Assert
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var loginResponseObject = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken);
        loginResponseObject.Should().NotBeNull();
        loginResponseObject.AccessToken.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenPayloadIsInvalid()
    {
        // Arrange
        const string invalidJson = """
                                   {
                                     "password": "P@ssw0rd123!"
                                   }
                                   """;

        using var content = new StringContent(invalidJson, System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(
            "/api/account/login",
            content,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var email = $"john.{Guid.NewGuid():N}@example.com";
        const string password = "P@ssw0rd123!";
        
        var register = new RegisterAccountRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = email,
            Password = password,
        };

        // Act
        var registerResponse = await _client.PostAsJsonAsync(
            "/api/account/register",
            register,
            TestContext.Current.CancellationToken);
        
        var login = new LoginAccountRequest
        {
            Email = $"not.john.{Guid.NewGuid():N}@example.com",
            Password = password,
        };
        
        var loginResponse = await _client.PostAsJsonAsync("/api/account/login", login,  TestContext.Current.CancellationToken);
        
        // Assert
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task GetUser_ShouldReturnUnauthorized_WhenNoToken()
    {
        // Act
        var response = await _client.GetAsync("/api/account/user", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUser_ShouldReturnOk_WhenUserRoleTokenProvided()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.User));

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/account/user");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAdmin_ShouldReturnForbidden_WhenUserRoleTokenProvided()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.User));

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/account/admin");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task GetById_ShouldReturnProblemDetails500_WhenUnhandledExceptionOccurs()
    {
        // Arrange
        var token = TestHelpers.CreateTestToken(nameof(Role.Moderator));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
        context.WifiLoginDetails.Add(new WifiLoginDetails
        {
            Id = 12345,
            Ssid = "broken",
            EncryptedPassword = "not-a-valid-protected-value",
            CreatedDate = DateTime.UtcNow
        });
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var response = await _client.GetAsync("/api/wifidetails/12345", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        body.Should().NotBeNull();
        body.Status.Should().Be(StatusCodes.Status500InternalServerError);
    }
    
}