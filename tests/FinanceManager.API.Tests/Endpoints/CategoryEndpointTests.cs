using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FinanceManager.API.Tests.Fixture;
using FinanceManager.Application.DTOs.Responses;
using FinanceManager.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceManager.API.Tests.Endpoints
{
    public class CategoryEndpointTests 
        : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly AppDbContext _context;

        public CategoryEndpointTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _scope = factory.Services.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }

        public async Task InitializeAsync()
        {
            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            _context.Categories.RemoveRange(_context.Categories);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task POST_ShouldCreateCategory()
        {
            var request = new
            {
                name = "Test",
                description = "Description"
            };

            var content = GetContent(request);

            var response = await _client.PostAsync("/api/categories", content);

            var category = await response.Content.ReadFromJsonAsync<CategoryResponse>();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(request.name, category!.Name);
        }

        [Fact]
        public async Task GET_WhenIdIsProvided_ShouldReturnCategory()
        {
            var request = new
            {
                name = "Test",
                description = "Description"
            };

            var content = GetContent(request);

            var result = await _client.PostAsync("/api/categories", content);

            var categoryResponse = await result.Content.ReadFromJsonAsync<CategoryResponse>();

            var response = await _client.GetAsync($"/api/categories/{categoryResponse!.Id}");

            var category = await response.Content.ReadFromJsonAsync<CategoryResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(categoryResponse.Id, category!.Id);
            Assert.Equal(categoryResponse.Name, category.Name);
            Assert.Equal(categoryResponse.Description, category.Description);
        }

        [Fact]
        public async Task GET_ShouldReturnAllCategories()
        {
            var request = new
            {
                name = "Test",
                description = "Description"
            };

            var content = GetContent(request);

            _ = await _client.PostAsync("/api/categories", content);

            var response = await _client.GetAsync("/api/categories/all");
            var categories = await response.Content.ReadFromJsonAsync<List<CategoryResponse>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEmpty(categories!);
            Assert.Single(categories!);
        }

        private StringContent GetContent(object request)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            return content;
        }        
    }
}
