using System.Text;
using API.Endpoints;
using API.Handlers;
using FinanceManager.API.Endpoints;
using FinanceManager.Application.Dispatchers;
using FinanceManager.Application.Handlers;
using FinanceManager.Application.Interfaces.Dispatchers;
using FinanceManager.Application.Interfaces.Handlers;
using FinanceManager.Domain.Events;
using FinanceManager.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace FinanceManager.API.Extensions
{
    public static class APIDependencyInjection
    {
        public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddScoped<IDomainEventHandler<TransactionPaidEvent>, TransactionPaidEventHandler>();
            services.AddScoped<IDomainEventHandler<TransactionReopenEvent>, TransactionReopenEventHandler>();
            services.AddScoped<IDomainEventHandler<TransactionCancelEvent>, TransactionCancelEventHandler>();
            services.AddProblemDetails();
            services.AddEndpointsApiExplorer();

            // Swagger com suporte a Bearer token
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Informe o token JWT. Exemplo: Bearer {seu_token}"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // JWT Bearer Authentication
            var jwtSection = configuration.GetSection("Jwt");
            var secretKey = jwtSection["SecretKey"]!;

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey  = true,
                        ValidIssuer = jwtSection["Issuer"],
                        ValidAudience = jwtSection["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });

            return services;
        }

        public static WebApplication Setup(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapAuthEndpoints();
            app.MapCategoryEndpoints();
            app.MapTransactionEndpoints();

            return app;
        }
    }
}
