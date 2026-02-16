using FinanceManager.API.Extensions;
using FinanceManager.Infrastructure.Extensions;
using FinanceManager.Application.Extensions;

var builder = WebApplication
    .CreateBuilder(args);

builder.Services
    .AddWebApi()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder
    .Build()
    .Setup();

app.Run();

public partial class Program { }