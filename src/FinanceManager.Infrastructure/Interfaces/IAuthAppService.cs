using FinanceManager.Infrastructure.DTOs;

namespace FinanceManager.Infrastructure.Interfaces;

public interface IAuthAppService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
