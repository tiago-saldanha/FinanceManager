using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Finanza.Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Finanza.Infrastructure.Services;

public class TokenService(IConfiguration configuration)
{
    public (string Token, DateTime ExpiresAt) GenerateToken(AppUser user)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var secretKey = jwtSection["SecretKey"]!;
        var issuer = jwtSection["Issuer"]!;
        var audience = jwtSection["Audience"]!;
        var expiresIn = int.Parse(jwtSection["ExpiresInMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiresIn);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
