namespace Finanza.Infrastructure.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetAsync(string toEmail, string resetLink);
}
