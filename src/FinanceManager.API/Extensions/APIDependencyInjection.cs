using API.Endpoints;
using API.Handlers;
using FinanceManager.Application.Dispatchers;
using FinanceManager.Application.Handlers;
using FinanceManager.Application.Interfaces.Dispatchers;
using FinanceManager.Application.Interfaces.Handlers;
using FinanceManager.Domain.Events;
using FinanceManager.Infrastructure.Data;

namespace FinanceManager.API.Extensions
{
    public static class APIDependencyInjection
    {
        public static IServiceCollection AddWebApi(this IServiceCollection services)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddScoped<IDomainEventHandler<TransactionPaidEvent>, TransactionPaidEventHandler>();
            services.AddScoped<IDomainEventHandler<TransactionReopenEvent>, TransactionReopenEventHandler>();
            services.AddScoped<IDomainEventHandler<TransactionCancelEvent>, TransactionCancelEventHandler>();
            services.AddProblemDetails();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

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

            app.MapCategoryEndpoints();
            app.MapTransactionEndpoints();

            return app;
        }
    }
}
