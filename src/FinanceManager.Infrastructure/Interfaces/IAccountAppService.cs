using FinanceManager.Infrastructure.DTOs;

namespace FinanceManager.Infrastructure.Interfaces;

public interface IAccountAppService
{
    Task<ExportDataResponse> ExportDataAsync(string userId);
    Task DeleteAccountAsync(string userId, string password);
}
