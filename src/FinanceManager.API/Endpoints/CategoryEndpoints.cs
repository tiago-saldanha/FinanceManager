using Application.DTOs.Requests;
using Application.Interfaces.Services;

namespace API.Endpoints
{
    public static class CategoryEndpoints
    {
        public static void MapCategoryEndpoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/api/categories");
            
            group.MapGet("/all", async (ICategoryAppService service) 
                => Results.Ok(await service.GetAllAsync()));
            
            group.MapGet("/{id:guid}", async (Guid id, ICategoryAppService service) 
                => Results.Ok(await service.GetByIdAsync(id)));
            
            group.MapPost("/", async (CategoryRequest request, ICategoryAppService service) =>
            {
                var result = await service.CreateAsync(request);
                return Results.Created($"/api/categories/{result.Id}", result);
            });
        }
    }
}
