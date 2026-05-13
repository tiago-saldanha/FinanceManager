namespace Finanza.Infrastructure.DTOs;

public record RegisterRequest(
    string FullName,
    string Email,
    string Password);
