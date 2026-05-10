using Microsoft.AspNetCore.Identity;

namespace FinanceManager.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
