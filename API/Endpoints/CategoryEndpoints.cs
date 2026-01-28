using API.Application.DTOs.Requests;
using API.Application.Services;

namespace API.EndPoints
{
    public static class CategoryEndpoints
    {
        public static void MapCategoryEndpoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/api/categories");
            
            group.MapGet("/all", async (CategoryService service) => Results.Ok(await service.GetAllAsync()));
            
            group.MapGet("/{id:guid}", async (Guid id, CategoryService service) => Results.Ok(await service.GetByIdAsync(id)));
            
            group.MapPost("/", async (CategoryRequest request, CategoryService service) =>
            {
                var result = await service.CreateAsync(request);
                return Results.Created($"/api/categories/{result.Id}", result);
            });
        }
    }
}
