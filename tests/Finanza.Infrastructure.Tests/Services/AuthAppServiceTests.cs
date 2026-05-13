using Finanza.Infrastructure.DTOs;
using Finanza.Infrastructure.Identity;
using Finanza.Infrastructure.Interfaces;
using Finanza.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Finanza.Infrastructure.Tests.Services;

public class AuthAppServiceTests
{
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly Mock<IEmailService>        _emailServiceMock;
    private readonly Mock<IConfiguration>       _configMock;
    private readonly AuthAppService             _sut;

    private static readonly AppUser TestUser = new()
    {
        Id = "test-id",
        Email = "test@test.com",
        UserName = "test@test.com",
        FullName = "Test User",
    };

    public AuthAppServiceTests()
    {
        var store = new Mock<IUserStore<AppUser>>();
        _userManagerMock  = new Mock<UserManager<AppUser>>(
            store.Object, null, null, null, null, null, null, null, null);
        _emailServiceMock = new Mock<IEmailService>();
        _configMock       = new Mock<IConfiguration>();

        _configMock.Setup(c => c["App:FrontendUrl"]).Returns("http://localhost:4200");

        _sut = new AuthAppService(
            _userManagerMock.Object,
            null!,
            null!,
            _emailServiceMock.Object,
            _configMock.Object);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenUserNotFound_ThrowsUnauthorizedAccessException()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _sut.ChangePasswordAsync("unknown-id", new ChangePasswordRequest("old", "new")));
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenCurrentPasswordIsWrong_ThrowsInvalidOperationException()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync(TestUser.Id)).ReturnsAsync(TestUser);
        _userManagerMock
            .Setup(m => m.ChangePasswordAsync(TestUser, "wrong", It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Incorrect password." }));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.ChangePasswordAsync(TestUser.Id, new ChangePasswordRequest("wrong", "New@Pass123")));
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenValid_CompletesSuccessfully()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync(TestUser.Id)).ReturnsAsync(TestUser);
        _userManagerMock
            .Setup(m => m.ChangePasswordAsync(TestUser, "old", "New@Pass123"))
            .ReturnsAsync(IdentityResult.Success);

        await _sut.ChangePasswordAsync(TestUser.Id, new ChangePasswordRequest("old", "New@Pass123"));

        _userManagerMock.Verify(m => m.ChangePasswordAsync(TestUser, "old", "New@Pass123"), Times.Once);
    }

    [Fact]
    public async Task ForgotPasswordAsync_WhenUserNotFound_ReturnsWithoutSendingEmail()
    {
        _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?)null);

        await _sut.ForgotPasswordAsync(new ForgotPasswordRequest("noexiste@test.com"));

        _emailServiceMock.Verify(m => m.SendPasswordResetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ForgotPasswordAsync_WhenUserExists_SendsEmailWithResetLink()
    {
        _userManagerMock.Setup(m => m.FindByEmailAsync(TestUser.Email!)).ReturnsAsync(TestUser);
        _userManagerMock.Setup(m => m.GeneratePasswordResetTokenAsync(TestUser)).ReturnsAsync("reset-token-123");

        await _sut.ForgotPasswordAsync(new ForgotPasswordRequest(TestUser.Email!));

        _emailServiceMock.Verify(
            m => m.SendPasswordResetAsync(
                TestUser.Email!,
                It.Is<string>(link => link.Contains("reset-token-123") && link.Contains("reset-password"))),
            Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_WhenUserNotFound_ThrowsInvalidOperationException()
    {
        _userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.ResetPasswordAsync(new ResetPasswordRequest("noexiste@test.com", "token", "New@Pass123")));
    }

    [Fact]
    public async Task ResetPasswordAsync_WhenTokenIsInvalid_ThrowsInvalidOperationException()
    {
        _userManagerMock.Setup(m => m.FindByEmailAsync(TestUser.Email!)).ReturnsAsync(TestUser);
        _userManagerMock
            .Setup(m => m.ResetPasswordAsync(TestUser, "invalid-token", It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Invalid token." }));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.ResetPasswordAsync(new ResetPasswordRequest(TestUser.Email!, "invalid-token", "New@Pass123")));
    }

    [Fact]
    public async Task ResetPasswordAsync_WhenValid_CompletesSuccessfully()
    {
        _userManagerMock.Setup(m => m.FindByEmailAsync(TestUser.Email!)).ReturnsAsync(TestUser);
        _userManagerMock
            .Setup(m => m.ResetPasswordAsync(TestUser, "valid-token", "New@Pass123"))
            .ReturnsAsync(IdentityResult.Success);

        await _sut.ResetPasswordAsync(new ResetPasswordRequest(TestUser.Email!, "valid-token", "New@Pass123"));

        _userManagerMock.Verify(m => m.ResetPasswordAsync(TestUser, "valid-token", "New@Pass123"), Times.Once);
    }
}
