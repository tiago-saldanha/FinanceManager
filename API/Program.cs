using API.Application.Services;
using API.Data;
using API.Endpoints;
using API.EndPoints;
using API.Infrastructure.Handlers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<CategoryService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapCategoryEndpoints();
app.MapTransactionEndpoints();

app.Run();
