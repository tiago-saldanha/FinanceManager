using Application.DTOs.Requests;
using Application.Enums;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints
{
    public static class TransactionEndpoints
    {
        public static void MapTransactionEndpoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/api/transactions");
            
            group.MapGet("/{id:guid}", async (Guid id, ITransactionAppService service) 
                => Results.Ok(await service.GetByIdAsync(id)));
            
            group.MapGet("/all", async (ITransactionAppService service) 
                => Results.Ok(await service.GetAllAsync()));

            group.MapGet("/status/{status}", async (ITransactionAppService service, TransactionStatusDto status) 
                => Results.Ok(await service.GetByStatusAsync(status)));

            group.MapGet("/type/{type}", async (ITransactionAppService service, TransactionTypeDto type) 
                => Results.Ok(await service.GetByTypeAsync(type)));
            
            group.MapPut("/pay/{id:guid}", async (Guid id, PayTransactionRequest request, ITransactionAppService service) 
                => Results.Ok(await service.PayAsync(id, request)));

            group.MapPut("/reopen/{id:Guid}", async (Guid id, ITransactionAppService service) 
                => Results.Ok(await service.ReopenAsync(id)));
            
            group.MapPut("/cancel/{id:guid}", async (Guid id, ITransactionAppService service) 
                => Results.Ok(await service.CancelAsync(id)));
            
            group.MapPost("/", async ([FromBody] CreateTransactionRequest request, ITransactionAppService service) =>
            {
                var result = await service.CreateAsync(request);
                return Results.Created($"/api/transactions/{result.Id}", result);
            });
        }
    }
}
