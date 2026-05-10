using FinanceManager.Domain.Repositories;
using FinanceManager.Infrastructure.Data;
using FinanceManager.Infrastructure.Identity;
using FinanceManager.Infrastructure.Interfaces;
using FinanceManager.Infrastructure.Repositories;
using FinanceManager.Infrastructure.Services;
using FinanceManager.Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceManager.Infrastructure.Extensions;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Banco compartilhado (Identity) ────────────────────────────────
        var sharedConnection = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(sharedConnection,
                b => b.MigrationsAssembly("FinanceManager.Infrastructure")));

        // ── ASP.NET Core Identity ─────────────────────────────────────────
        services
            .AddIdentityCore<AppUser>(options =>
            {
                options.Password.RequireDigit           = true;
                options.Password.RequireLowercase       = true;
                options.Password.RequireUppercase       = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength         = 6;
                options.User.RequireUniqueEmail         = true;
            })
            .AddEntityFrameworkStores<AppDbContext>();

        // ── Multi-tenancy ─────────────────────────────────────────────────
        services.AddScoped<ITenantConnectionResolver, TenantConnectionResolver>();
        services.AddScoped<TenantProvisionerService>();

        // TenantDbContext com connection string resolvida por request
        services.AddDbContext<TenantDbContext>((sp, options) =>
        {
            var resolver = sp.GetRequiredService<ITenantConnectionResolver>();
            options.UseSqlite(resolver.GetConnectionString());
        });

        // ── Serviços de autenticação ──────────────────────────────────────
        services.AddScoped<TokenService>();
        services.AddScoped<IAuthAppService, AuthAppService>();

        // ── Repositórios ──────────────────────────────────────────────────
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;
    }
}
