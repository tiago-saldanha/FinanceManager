using Microsoft.AspNetCore.Identity;

namespace Finanza.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
