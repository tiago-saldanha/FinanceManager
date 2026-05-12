using System.Reflection;
using System.Text;
using System.Text.Json;
using FinanceManager.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FinanceManager.Infrastructure.Services;

public class ResendEmailService(HttpClient httpClient, IConfiguration configuration) : IEmailService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task SendPasswordResetAsync(string toEmail, string resetLink)
    {
        var appSection = configuration.GetSection("App");
        var fromEmail = appSection["FromEmail"]!;
        var fromName = appSection["FromName"]!;

        var body = new
        {
            from = $"{fromName} <{fromEmail}>",
            to = new[] { toEmail },
            subject = "Redefinição de senha — Finance Manager",
            html = await BuildEmailHtmlAsync(resetLink)
        };

        var json = JsonSerializer.Serialize(body, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("https://api.resend.com/emails", content);
        response.EnsureSuccessStatusCode();
    }

    private static async Task<string> BuildEmailHtmlAsync(string resetLink)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "FinanceManager.Infrastructure.Templates.password-reset.html";

        await using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Template '{resourceName}' não encontrado.");

        using var reader  = new StreamReader(stream);
        var template = await reader.ReadToEndAsync();

        return template.Replace("{{resetLink}}", resetLink);
    }
}
