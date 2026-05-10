using FinanceManager.Infrastructure.DTOs;
using FinanceManager.Infrastructure.Interfaces;

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
    }
}
