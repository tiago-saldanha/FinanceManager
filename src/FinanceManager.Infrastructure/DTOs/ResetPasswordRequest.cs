namespace FinanceManager.Infrastructure.DTOs;

public record ResetPasswordRequest(string Email, string Token, string NewPassword);
