using Finanza.Domain.Repositories;
using Finanza.Infrastructure.Data;
using Finanza.Infrastructure.Identity;
using Finanza.Infrastructure.Interfaces;
using Finanza.Infrastructure.Repositories;
using Finanza.Infrastructure.Services;
using Finanza.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Finanza.Infrastructure.Extensions;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // â”€â”€ Banco compartilhado (Identity) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        var sharedConnection = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(sharedConnection,
                b => b.MigrationsAssembly("Finanza.Infrastructure")));

        // â”€â”€ ASP.NET Core Identity â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        services
            .AddIdentityCore<AppUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders()
            .AddErrorDescriber<PortugueseIdentityErrorDescriber>();

        // â”€â”€ Multi-tenancy â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        services.AddScoped<ITenantConnectionResolver, TenantConnectionResolver>();
        services.AddScoped<TenantProvisionerService>();

        // TenantDbContext com connection string resolvida por request
        services.AddDbContext<TenantDbContext>((sp, options) =>
        {
            var resolver = sp.GetRequiredService<ITenantConnectionResolver>();
            options.UseSqlite(resolver.GetConnectionString());
        });

        // â”€â”€ ServiÃ§os de autenticaÃ§Ã£o â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        services.AddScoped<TokenService>();
        services.AddScoped<IAuthAppService, AuthAppService>();
        services.AddScoped<IAccountAppService, AccountAppService>();

        // â”€â”€ E-mail (Resend) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        services.AddHttpClient<IEmailService, ResendEmailService>((sp, client) =>
        {
            var apiKey = configuration["Resend:ApiKey"]!;
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        });

        // â”€â”€ RepositÃ³rios â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;
    }
}
