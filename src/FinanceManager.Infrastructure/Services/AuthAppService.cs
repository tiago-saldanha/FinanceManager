using FinanceManager.Infrastructure.DTOs;
using FinanceManager.Infrastructure.Identity;
using FinanceManager.Infrastructure.Interfaces;
using FinanceManager.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;

namespace FinanceManager.Infrastructure.Services;

public class AuthAppService(
    UserManager<AppUser> userManager,
    TokenService tokenService,
    TenantProvisionerService tenantProvisioner) : IAuthAppService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            throw new InvalidOperationException($"E-mail '{request.Email}' já está em uso.");

        var user = new AppUser
        {
            FullName = request.FullName,
            Email    = request.Email,
            UserName = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Falha ao criar usuário: {errors}");
        }

        tenantProvisioner.ProvisionTenant(user.Id);

        var (token, expiresAt) = tokenService.GenerateToken(user);
        return new AuthResponse(token, user.Email!, user.FullName, expiresAt);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Credenciais inválidas.");

        var validPassword = await userManager.CheckPasswordAsync(user, request.Password);
        if (!validPassword)
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        tenantProvisioner.ProvisionTenant(user.Id);

        var (token, expiresAt) = tokenService.GenerateToken(user);
        return new AuthResponse(token, user.Email!, user.FullName, expiresAt);
    }
}
