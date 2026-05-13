using Finanza.API.Extensions;
using Finanza.Infrastructure.Extensions;
using Finanza.Application.Extensions;

var builder = WebApplication
    .CreateBuilder(args);

builder.Services
    .AddWebApi(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder
    .Build()
    .Setup();

app.Run();

public partial class Program { }