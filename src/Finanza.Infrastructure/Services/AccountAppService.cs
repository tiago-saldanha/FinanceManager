using Finanza.Infrastructure.Data;
using Finanza.Infrastructure.DTOs;
using Finanza.Infrastructure.Identity;
using Finanza.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Finanza.Infrastructure.Services;

public class AccountAppService(
    UserManager<AppUser> userManager,
    TenantDbContext tenantDb,
    IConfiguration configuration) : IAccountAppService
{
    public async Task<ExportDataResponse> ExportDataAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new UnauthorizedAccessException("UsuÃ¡rio nÃ£o encontrado.");

        var categories = await tenantDb.Categories
            .AsNoTracking()
            .Select(c => new CategoriaExport(c.Name.ToString(), c.Description))
            .ToListAsync();

        var transactions = await tenantDb.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Select(t => new TransacaoExport(
                t.Description.ToString(),
                t.Amount.Value,
                t.Type.ToString(),
                t.Status.ToString(),
                t.Category != null ? t.Category.Name.ToString() : "Sem Categoria",
                t.Dates.DueDate,
                t.PaymentDate
            ))
            .ToListAsync();

        return new ExportDataResponse(
            DateTime.UtcNow,
            new PerfilExport(user.FullName, user.Email!),
            categories,
            transactions
        );
    }

    public async Task DeleteAccountAsync(string userId, string password)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new UnauthorizedAccessException("UsuÃ¡rio nÃ£o encontrado.");

        var validPassword = await userManager.CheckPasswordAsync(user, password);
        if (!validPassword)
            throw new InvalidOperationException("Senha incorreta.");

        tenantDb.Transactions.RemoveRange(tenantDb.Transactions);
        tenantDb.Categories.RemoveRange(tenantDb.Categories);
        await tenantDb.SaveChangesAsync();

        await tenantDb.DisposeAsync();
        SqliteConnection.ClearAllPools();

        var baseFolder = configuration["TenantDb:BaseFolder"]!;
        var dbPath = Path.Combine(baseFolder, $"user_{userId}.db");
        if (File.Exists(dbPath)) File.Delete(dbPath);
        if (File.Exists(dbPath + "-wal")) File.Delete(dbPath + "-wal");
        if (File.Exists(dbPath + "-shm")) File.Delete(dbPath + "-shm");

        await userManager.DeleteAsync(user);
    }
}
