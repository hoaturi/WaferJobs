using FluentAssertions;
using JobBoard.Domain.Auth;
using JobBoard.Features.Auth.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test;

public class ResetPasswordCommandHandlerTests
{
    private readonly ResetPasswordCommandHandler _handler;
    private readonly Mock<ILogger<ResetPasswordCommandHandler>> _mockLogger;
    private readonly Mock<UserManager<ApplicationUserEntity>> _mockUserManager;
    private readonly Mock<IUserStore<ApplicationUserEntity>> _mockUserStore;

    public ResetPasswordCommandHandlerTests()
    {
        _mockUserStore = new Mock<IUserStore<ApplicationUserEntity>>();
        _mockUserManager = new Mock<UserManager<ApplicationUserEntity>>(
            _mockUserStore.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!
        );
        _mockLogger = new Mock<ILogger<ResetPasswordCommandHandler>>();

        _handler = new ResetPasswordCommandHandler(_mockUserManager.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task WhenSuccessful_ShouldResetPassword_And_ReturnUnit()
    {
        // Arrange
        var request = new ResetPasswordCommand(
            Guid.NewGuid(),
            "resetToken",
            "newPassword",
            "newPassword"
        );

        var user = new ApplicationUserEntity { Id = request.UserId };

        _mockUserManager.Setup(x => x.FindByIdAsync(request.UserId.ToString())).ReturnsAsync(user);

        _mockUserManager
            .Setup(x => x.ResetPasswordAsync(user, request.Token, request.Password))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<Unit>();

        _mockUserManager.Verify(x => x.FindByIdAsync(request.UserId.ToString()), Times.Once);
        _mockUserManager.Verify(
            x => x.ResetPasswordAsync(user, request.Token, request.Password),
            Times.Once
        );
    }

    [Fact]
    public async Task WhenUserNotFound_ShouldReturnUserNotFoundError()
    {
        // Arrange
        var request = new ResetPasswordCommand(
            Guid.NewGuid(),
            "resetToken",
            "newPassword",
            "newPassword"
        );

        _mockUserManager
            .Setup(x => x.FindByIdAsync(request.UserId.ToString()))
            .ReturnsAsync(default(ApplicationUserEntity));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AuthErrors.UserNotFound);

        _mockUserManager.Verify(x => x.FindByIdAsync(request.UserId.ToString()), Times.Once);
        _mockUserManager.Verify(
            x =>
                x.ResetPasswordAsync(
                    It.IsAny<ApplicationUserEntity>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ),
            Times.Never
        );
    }

    [Fact]
    public async Task WhenResetFails_ShouldReturnInvalidTokenError()
    {
        // Arrange
        var request = new ResetPasswordCommand(
            Guid.NewGuid(),
            "resetToken",
            "newPassword",
            "newPassword"
        );

        var user = new ApplicationUserEntity { Id = request.UserId };

        _mockUserManager.Setup(x => x.FindByIdAsync(request.UserId.ToString())).ReturnsAsync(user);

        _mockUserManager
            .Setup(x => x.ResetPasswordAsync(user, request.Token, request.Password))
            .ReturnsAsync(IdentityResult.Failed());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(AuthErrors.InvalidToken, result.Error);

        _mockUserManager.Verify(x => x.FindByIdAsync(request.UserId.ToString()), Times.Once);
        _mockUserManager.Verify(
            x => x.ResetPasswordAsync(user, request.Token, request.Password),
            Times.Once
        );
    }
}