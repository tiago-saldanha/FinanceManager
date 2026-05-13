using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Finanza.Infrastructure.Tenancy;

public class TenantConnectionResolver(
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration) : ITenantConnectionResolver
{
    public string GetConnectionString()
    {
        var user = httpContextAccessor.HttpContext?.User;

        var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user?.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("UsuÃ¡rio nÃ£o autenticado.");

        var baseFolder = configuration["TenantDb:BaseFolder"]!;
        Directory.CreateDirectory(baseFolder);

        return $"Data Source={Path.Combine(baseFolder, $"user_{userId}.db")}";
    }
}
