using API.Endpoints;
using API.Handlers;
using FinanceManager.Application.Dispatchers;
using FinanceManager.Application.Handlers;
using FinanceManager.Application.Interfaces.Dispatchers;
using FinanceManager.Application.Interfaces.Handlers;
using FinanceManager.Domain.Events;

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
            return services;
        }

        public static WebApplication Setup(this WebApplication app)
        {
            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapCategoryEndpoints();
            app.MapTransactionEndpoints();

            return app;
        }
    }
}
