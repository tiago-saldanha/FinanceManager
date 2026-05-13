using Finanza.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Data;

/// <summary>
/// Banco compartilhado â€” contÃ©m apenas as tabelas do ASP.NET Core Identity.
/// Os dados financeiros de cada usuÃ¡rio ficam em seu prÃ³prio TenantDbContext.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<AppUser>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => base.OnModelCreating(modelBuilder);
}
