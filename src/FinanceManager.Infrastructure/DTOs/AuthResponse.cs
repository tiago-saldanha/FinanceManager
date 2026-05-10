namespace FinanceManager.Infrastructure.DTOs;

public record AuthResponse(
    string Token,
    string Email,
    string FullName,
    DateTime ExpiresAt);
