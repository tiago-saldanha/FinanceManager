namespace FinanceManager.Infrastructure.DTOs;

public record LoginRequest(
    string Email,
    string Password);
