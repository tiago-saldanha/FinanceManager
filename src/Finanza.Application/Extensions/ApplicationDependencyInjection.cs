using Finanza.Application.Interfaces.Services;
using Finanza.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Finanza.Application.Extensions
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
