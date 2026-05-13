using Finanza.Infrastructure.DTOs;

namespace Finanza.Infrastructure.Interfaces;

public interface IAccountAppService
{
    Task<ExportDataResponse> ExportDataAsync(string userId);
    Task DeleteAccountAsync(string userId, string password);
}
