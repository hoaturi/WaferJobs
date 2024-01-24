using FluentAssertions;
using JobBoard;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Test;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<IUserStore<ApplicationUser>> _mockUserStore;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly ResetPasswordCommandHandler _handler;

    public ResetPasswordCommandHandlerTests()
    {
        _mockUserStore = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
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

        _handler = new ResetPasswordCommandHandler(_mockUserManager.Object);
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

        var user = new ApplicationUser { Id = request.UserId };

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
            .ReturnsAsync(default(ApplicationUser));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AuthErrors.UserNotFound);

        _mockUserManager.Verify(x => x.FindByIdAsync(request.UserId.ToString()), Times.Once);
        _mockUserManager.Verify(
            x =>
                x.ResetPasswordAsync(
                    It.IsAny<ApplicationUser>(),
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

        var user = new ApplicationUser { Id = request.UserId };

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
