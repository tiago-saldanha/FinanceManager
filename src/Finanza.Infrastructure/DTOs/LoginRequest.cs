namespace Finanza.Infrastructure.DTOs;

public record LoginRequest(
    string Email,
    string Password);
