namespace FinanceManager.Infrastructure.Tenancy;

public interface ITenantConnectionResolver
{
    string GetConnectionString();
}
