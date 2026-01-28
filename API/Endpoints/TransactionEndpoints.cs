using API.Application.DTOs.Requests;
using API.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints
{
    public static class TransactionEndpoints
    {
        public static void MapTransactionEndpoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/api/transactions");
            
            group.MapGet("/{id:guid}", async (Guid id, TransactionService service) => Results.Ok(await service.GetByIdAsync(id)));
            
            group.MapGet("/all", async (TransactionService service) => Results.Ok(await service.GetAllAsync()));

            group.MapGet("/status/{status}", async (TransactionService service, string status) => Results.Ok(await service.GetByStatusAsync(status)));

            group.MapGet("/type/{type}", async (TransactionService service, string type) => Results.Ok(await service.GetByTypeAsync(type)));
            
            group.MapPut("/pay/{id:guid}", async (Guid id, PayTransactionRequest request, TransactionService service) => Results.Ok(await service.PaidAsync(request)));
            
            group.MapPut("/cancel/{id:guid}", async (Guid id, TransactionService service) => Results.Ok(await service.CancelAsync(id)));
            
            group.MapPost("/", async ([FromBody] CreateTransactionRequest request, TransactionService service) =>
            {
                var result = await service.CreateAsync(request);
                return Results.Created($"/api/transactions/{result.Id}", result);
            });
        }
    }
}
