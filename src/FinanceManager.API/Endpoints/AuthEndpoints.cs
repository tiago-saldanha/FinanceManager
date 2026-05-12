using System.Security.Claims;
using FinanceManager.Infrastructure.DTOs;
using FinanceManager.Infrastructure.Interfaces;
using Microsoft.IdentityModel.JsonWebTokens;

namespace FinanceManager.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", async (RegisterRequest request, IAuthAppService service) =>
        {
            var result = await service.RegisterAsync(request);
            return Results.Created("/api/auth/login", result);
        })
        .WithSummary("Registrar novo usuário")
        .AllowAnonymous();

        group.MapPost("/login", async (LoginRequest request, IAuthAppService service) =>
        {
            var result = await service.LoginAsync(request);
            return Results.Ok(result);
        })
        .WithSummary("Autenticar e obter JWT")
        .AllowAnonymous();

        group.MapPut("/change-password", async (
            ChangePasswordRequest request,
            ClaimsPrincipal principal,
            IAuthAppService service) =>
        {
            var userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)!;
            await service.ChangePasswordAsync(userId, request);
            return Results.NoContent();
        })
        .WithSummary("Alterar senha do usuário autenticado")
        .RequireAuthorization();

        group.MapPost("/forgot-password", async (ForgotPasswordRequest request, IAuthAppService service) =>
        {
            await service.ForgotPasswordAsync(request);
            return Results.NoContent();
        })
        .WithSummary("Solicitar redefinição de senha por e-mail")
        .AllowAnonymous();

        group.MapPost("/reset-password", async (ResetPasswordRequest request, IAuthAppService service) =>
        {
            await service.ResetPasswordAsync(request);
            return Results.NoContent();
        })
        .WithSummary("Redefinir senha com token recebido por e-mail")
        .AllowAnonymous();
    }
}
