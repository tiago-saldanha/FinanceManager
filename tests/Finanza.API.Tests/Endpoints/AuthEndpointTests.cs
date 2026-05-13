using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using Finanza.API.Tests.Fixture;

namespace Finanza.API.Tests.Endpoints;

[Collection("Auth")]
public class AuthEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly HttpClient _authClient;

    public AuthEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory    = factory;
        _client     = factory.CreateClient();
        _authClient = factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task ChangePassword_Unauthenticated_Returns401()
    {
        var response = await _client.PutAsJsonAsync("/api/auth/change-password",
            new { currentPassword = CustomWebApplicationFactory.TestUserPassword, newPassword = "New@Pass123" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_WrongCurrentPassword_ReturnsError()
    {
        var response = await _authClient.PutAsJsonAsync("/api/auth/change-password",
            new { currentPassword = "WrongPassword@99", newPassword = "New@Pass123" });

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task ChangePassword_ValidCredentials_Returns204()
    {
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register",
            new { fullName = "Change Pass User", email = "changepass@test.com", password = "Test@123" });
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var auth  = await registerResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = auth.GetProperty("token").GetString()!;

        using var freshClient = _factory.CreateClient();
        freshClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await freshClient.PutAsJsonAsync("/api/auth/change-password",
            new { currentPassword = "Test@123", newPassword = "New@Pass123" });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ForgotPassword_ExistingEmail_Returns204()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/forgot-password",
            new { email = CustomWebApplicationFactory.TestUserEmail });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.NotNull(_factory.EmailStub.LastResetLink);
    }

    [Fact]
    public async Task ForgotPassword_NonExistingEmail_Returns204()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/forgot-password",
            new { email = "noexiste@test.com" });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_InvalidToken_ReturnsError()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/reset-password",
            new { email = CustomWebApplicationFactory.TestUserEmail, token = "token-invalido", newPassword = "New@Pass123" });

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task ResetPassword_ValidToken_Returns204()
    {
        await _client.PostAsJsonAsync("/api/auth/forgot-password",
            new { email = CustomWebApplicationFactory.TestUserEmail });

        var resetLink = _factory.EmailStub.LastResetLink!;
        var query     = HttpUtility.ParseQueryString(new Uri(resetLink).Query);
        var token     = query["token"]!;
        var email     = query["email"]!;

        var response = await _client.PostAsJsonAsync("/api/auth/reset-password",
            new { email, token, newPassword = "Reset@Pass123" });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
