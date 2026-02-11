using Application.Interfaces.Services;
using Application.Services;
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
/*
 * 
 *          builder.Services.AddProblemDetails();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var connection = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connection, b => b.MigrationsAssembly("FinanceManager.Infrastructure")));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ITransactionAppService, TransactionAppService>();
            builder.Services.AddScoped<ICategoryAppService, CategoryAppService>();
 */
