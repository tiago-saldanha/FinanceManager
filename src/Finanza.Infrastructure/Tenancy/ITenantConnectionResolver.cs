namespace Finanza.Infrastructure.Tenancy;

public interface ITenantConnectionResolver
{
    string GetConnectionString();
}
