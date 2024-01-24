using FluentAssertions;
using JobBoard;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Test;

public class ForgotPasswordCommandHandlerTest
{
    private readonly Mock<IUserStore<ApplicationUser>> _mockUserStore;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<IEmailSender> _mockEmailSender;
    private readonly ForgotPasswordCommandHandler _handler;

    public ForgotPasswordCommandHandlerTest()
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
        _mockEmailSender = new Mock<IEmailSender>();

        _handler = new ForgotPasswordCommandHandler(
            _mockUserManager.Object,
            _mockEmailSender.Object
        );
    }

    public async Task WhenSuccessful_ShouldSendEmail_And_ReturnUnit()
    {
        // Arrange
        var request = new ForgotPasswordCommand("test@test.com");
        var user = new ApplicationUser { Email = request.Email };
        var token = "resetToken";

        _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("token");
        _mockEmailSender.Setup(x => x.SendPasswordResetEmailAsync(new EmailDto(user, token)));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<Unit>();
        _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
    }

    public async Task WhenUserNotFound_ShouldReturnUserNotFoundError()
    {
        // Arrange
        var request = new ForgotPasswordCommand("test@test.com");

        _mockUserManager
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(default(ApplicationUser));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AuthErrors.UserNotFound);
        _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
        _mockUserManager.Verify(
            x => x.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()),
            Times.Never
        );
    }
}
