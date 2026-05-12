using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using FinanceManager.Infrastructure.Data;
using FinanceManager.Infrastructure.Identity;
using FinanceManager.Infrastructure.Interfaces;
using FinanceManager.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FinanceManager.API.Tests.Fixture;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    internal const string SecretKey = "65a0be4c-5f49-4f14-a720-1fd6e713691b";
    internal const string Issuer = "FinanceManager.API.Tests";
    internal const string Audience = "FinanceManager.Client.Tests";
    public   const string TestUserId = "test-user-id-00000000-0000-0000-0001";
    public   const string TestUserEmail = "test@financemanager.com";
    public   const string TestUserPassword = "Test@123";

    public EmailServiceStub EmailStub { get; } = new();

    private readonly SqliteConnection _identityConnection = new("DataSource=:memory:");
    private readonly SqliteConnection _tenantConnection   = new("DataSource=:memory:");

    public CustomWebApplicationFactory()
    {
        _identityConnection.Open();
        _tenantConnection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] = SecretKey,
                ["Jwt:Issuer"] = Issuer,
                ["Jwt:Audience"] = Audience,
                ["Jwt:ExpiresInMinutes"] = "60",
                ["TenantDb:BaseFolder"] = Path.GetTempPath(),
                ["App:FrontendUrl"] = "http://localhost:4200",
            });
        });

        builder.ConfigureServices(services =>
        {
            RemoveService<DbContextOptions<AppDbContext>>(services);
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(_identityConnection));

            RemoveService<DbContextOptions<TenantDbContext>>(services);
            RemoveService<TenantDbContext>(services);
            services.AddDbContext<TenantDbContext>(options =>
                options.UseSqlite(_tenantConnection));

            RemoveService<ITenantConnectionResolver>(services);
            services.AddScoped<ITenantConnectionResolver, StubTenantConnectionResolver>();

            RemoveService<IEmailService>(services);
            services.AddSingleton<IEmailService>(EmailStub);

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
            scope.ServiceProvider.GetRequiredService<TenantDbContext>().Database.EnsureCreated();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var testUser = new AppUser
            {
                Id       = TestUserId,
                UserName = TestUserEmail,
                Email    = TestUserEmail,
                FullName = "Test User",
            };
            userManager.CreateAsync(testUser, TestUserPassword).GetAwaiter().GetResult();
        });
    }

    public HttpClient CreateAuthenticatedClient(string userId = TestUserId)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", GenerateJwtToken(userId));
        return client;
    }

    private static string GenerateJwtToken(string userId)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, TestUserEmail),
            new Claim(JwtRegisteredClaimNames.Name, "Test User"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static void RemoveService<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(T));
        if (descriptor is not null) services.Remove(descriptor);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing) return;
        _identityConnection.Dispose();
        _tenantConnection.Dispose();
    }
}

public sealed class EmailServiceStub : IEmailService
{
    public string? LastResetLink { get; private set; }

    public Task SendPasswordResetAsync(string toEmail, string resetLink)
    {
        LastResetLink = resetLink;
        return Task.CompletedTask;
    }
}

file sealed class StubTenantConnectionResolver : ITenantConnectionResolver
{
    public string GetConnectionString() => "DataSource=:memory:";
}
