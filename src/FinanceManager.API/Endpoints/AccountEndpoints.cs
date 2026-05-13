using System.Security.Claims;
using FinanceManager.Infrastructure.DTOs;
using FinanceManager.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace FinanceManager.API.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/account")
            .WithTags("Account")
            .RequireAuthorization();

        group.MapGet("/export", async (ClaimsPrincipal principal, IAccountAppService service) =>
        {
            var userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)!;
            var data = await service.ExportDataAsync(userId);
            return Results.Ok(data);
        })
        .WithSummary("Exportar todos os dados do usuário (LGPD)");

        group.MapDelete("/", async (
            [FromBody] DeleteAccountRequest request,
            ClaimsPrincipal principal,
            IAccountAppService service) =>
        {
            var userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)!;
            await service.DeleteAccountAsync(userId, request.Password);
            return Results.NoContent();
        })
        .WithSummary("Cancelar conta e excluir todos os dados do usuário");
    }
}
