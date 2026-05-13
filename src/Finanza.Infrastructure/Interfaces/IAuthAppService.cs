using Finanza.Infrastructure.DTOs;

namespace Finanza.Infrastructure.Interfaces;

public interface IAuthAppService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task ChangePasswordAsync(string userId, ChangePasswordRequest request);
    Task ForgotPasswordAsync(ForgotPasswordRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
}
