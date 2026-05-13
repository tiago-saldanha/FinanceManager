using Microsoft.AspNetCore.Identity;

namespace Finanza.Infrastructure.Identity;

public class PortugueseIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DefaultError()
        => new() { Code = nameof(DefaultError), Description = "Ocorreu um erro desconhecido." };

    public override IdentityError ConcurrencyFailure()
        => new() { Code = nameof(ConcurrencyFailure), Description = "Falha de concorrÃªncia otimista. O objeto foi modificado." };

    public override IdentityError PasswordMismatch()
        => new() { Code = nameof(PasswordMismatch), Description = "Senha incorreta." };

    public override IdentityError InvalidToken()
        => new() { Code = nameof(InvalidToken), Description = "Token invÃ¡lido." };

    public override IdentityError LoginAlreadyAssociated()
        => new() { Code = nameof(LoginAlreadyAssociated), Description = "JÃ¡ existe um usuÃ¡rio com este login." };

    public override IdentityError InvalidUserName(string? userName)
        => new() { Code = nameof(InvalidUserName), Description = $"O nome de usuÃ¡rio '{userName}' Ã© invÃ¡lido. Use apenas letras e dÃ­gitos." };

    public override IdentityError InvalidEmail(string? email)
        => new() { Code = nameof(InvalidEmail), Description = $"O e-mail '{email}' Ã© invÃ¡lido." };

    public override IdentityError DuplicateUserName(string userName)
        => new() { Code = nameof(DuplicateUserName), Description = $"O nome de usuÃ¡rio '{userName}' jÃ¡ estÃ¡ em uso." };

    public override IdentityError DuplicateEmail(string email)
        => new() { Code = nameof(DuplicateEmail), Description = $"O e-mail '{email}' jÃ¡ estÃ¡ cadastrado." };

    public override IdentityError InvalidRoleName(string? role)
        => new() { Code = nameof(InvalidRoleName), Description = $"O nome de perfil '{role}' Ã© invÃ¡lido." };

    public override IdentityError DuplicateRoleName(string role)
        => new() { Code = nameof(DuplicateRoleName), Description = $"O perfil '{role}' jÃ¡ existe." };

    public override IdentityError UserAlreadyHasPassword()
        => new() { Code = nameof(UserAlreadyHasPassword), Description = "O usuÃ¡rio jÃ¡ possui uma senha definida." };

    public override IdentityError UserLockoutNotEnabled()
        => new() { Code = nameof(UserLockoutNotEnabled), Description = "O bloqueio de conta nÃ£o estÃ¡ habilitado para este usuÃ¡rio." };

    public override IdentityError UserAlreadyInRole(string role)
        => new() { Code = nameof(UserAlreadyInRole), Description = $"O usuÃ¡rio jÃ¡ pertence ao perfil '{role}'." };

    public override IdentityError UserNotInRole(string role)
        => new() { Code = nameof(UserNotInRole), Description = $"O usuÃ¡rio nÃ£o pertence ao perfil '{role}'." };

    public override IdentityError PasswordTooShort(int length)
        => new() { Code = nameof(PasswordTooShort), Description = $"A senha deve ter no mÃ­nimo {length} caracteres." };

    public override IdentityError PasswordRequiresNonAlphanumeric()
        => new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "A senha deve conter pelo menos um caractere especial (ex: !@#$%)." };

    public override IdentityError PasswordRequiresDigit()
        => new() { Code = nameof(PasswordRequiresDigit), Description = "A senha deve conter pelo menos um nÃºmero (0-9)." };

    public override IdentityError PasswordRequiresLower()
        => new() { Code = nameof(PasswordRequiresLower), Description = "A senha deve conter pelo menos uma letra minÃºscula (a-z)." };

    public override IdentityError PasswordRequiresUpper()
        => new() { Code = nameof(PasswordRequiresUpper), Description = "A senha deve conter pelo menos uma letra maiÃºscula (A-Z)." };
}
