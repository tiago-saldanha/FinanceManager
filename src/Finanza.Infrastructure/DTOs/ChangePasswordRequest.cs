namespace Finanza.Infrastructure.DTOs;

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
