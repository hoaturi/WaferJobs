using FluentAssertions;
using JobBoard.Common.Interfaces;
using JobBoard.Domain.Auth;
using JobBoard.Features.Auth.ForgotPassword;
using JobBoard.Infrastructure.Services.EmailService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test;

public class ForgotPasswordCommandHandlerTest
{
    private readonly ForgotPasswordCommandHandler _handler;
    private readonly Mock<IEmailService> _mockEmailSender;
    private readonly Mock<ILogger<ForgotPasswordCommandHandler>> _mockLogger;
    private readonly Mock<UserManager<ApplicationUserEntity>> _mockUserManager;
    private readonly Mock<IUserStore<ApplicationUserEntity>> _mockUserStore;

    public ForgotPasswordCommandHandlerTest()
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
        _mockEmailSender = new Mock<IEmailService>();
        _mockLogger = new Mock<ILogger<ForgotPasswordCommandHandler>>();

        _handler = new ForgotPasswordCommandHandler(
            _mockUserManager.Object,
            _mockEmailSender.Object,
            _mockLogger.Object
        );
    }

    public async Task WhenSuccessful_ShouldSendEmail_And_ReturnUnit()
    {
        // Arrange
        var request = new ForgotPasswordCommand("test@test.com");
        var user = new ApplicationUserEntity { Email = request.Email };
        var token = "resetToken";

        _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("token");
        _mockEmailSender.Setup(x => x.SendAsync(new PasswordResetDto(user, token)));

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
            .ReturnsAsync(default(ApplicationUserEntity));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AuthErrors.UserNotFound);
        _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
        _mockUserManager.Verify(
            x => x.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUserEntity>()),
            Times.Never
        );
    }
}