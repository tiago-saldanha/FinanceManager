using FinanceManager.Application.Interfaces.Services;
using FinanceManager.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceManager.Application.Extensions
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ITransactionAppService, TransactionAppService>();
            services.AddScoped<ICategoryAppService, CategoryAppService>();

            return services;
        }
    }
}
