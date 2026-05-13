using Finanza.Infrastructure.DTOs;
using Finanza.Infrastructure.Identity;
using Finanza.Infrastructure.Interfaces;
using Finanza.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Finanza.Infrastructure.Services;

public class AuthAppService(
    UserManager<AppUser> userManager,
    TokenService tokenService,
    TenantProvisionerService tenantProvisioner,
    IEmailService emailService,
    IConfiguration configuration) : IAuthAppService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            throw new InvalidOperationException($"E-mail '{request.Email}' jÃ¡ estÃ¡ em uso.");

        var user = new AppUser
        {
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Falha ao criar usuÃ¡rio: {errors}");
        }

        tenantProvisioner.ProvisionTenant(user.Id);

        var (token, expiresAt) = tokenService.GenerateToken(user);
        return new AuthResponse(token, user.Email!, user.FullName, expiresAt);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Credenciais invÃ¡lidas.");

        var validPassword = await userManager.CheckPasswordAsync(user, request.Password);
        if (!validPassword)
            throw new UnauthorizedAccessException("Credenciais invÃ¡lidas.");

        tenantProvisioner.ProvisionTenant(user.Id);

        var (token, expiresAt) = tokenService.GenerateToken(user);
        return new AuthResponse(token, user.Email!, user.FullName, expiresAt);
    }

    public async Task ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new UnauthorizedAccessException("UsuÃ¡rio nÃ£o encontrado.");

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Falha ao alterar senha: {errors}");
        }
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null) return;

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);
        var frontendUrl = configuration["App:FrontendUrl"]!;
        var resetLink = $"{frontendUrl}/reset-password?token={encodedToken}&email={Uri.EscapeDataString(request.Email)}";

        await emailService.SendPasswordResetAsync(request.Email, resetLink);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new InvalidOperationException("SolicitaÃ§Ã£o invÃ¡lida.");

        var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Falha ao redefinir senha: {errors}");
        }
    }
}
