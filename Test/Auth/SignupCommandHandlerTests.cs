using FluentAssertions;
using JobBoard.Common.Constants;
using JobBoard.Domain.Auth;
using JobBoard.Features.Auth.Signup;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test;

public class SignupCommandHandlerTests
{
    private readonly SignUpCommandHandler _handler;
    private readonly Mock<ILogger<SignUpCommandHandler>> _mockLogger;
    private readonly Mock<UserManager<ApplicationUserEntity>> _mockUserManager;
    private readonly Mock<IUserStore<ApplicationUserEntity>> _mockUserStore;

    public SignupCommandHandlerTests()
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
        _mockLogger = new Mock<ILogger<SignUpCommandHandler>>();

        _handler = new SignUpCommandHandler(_mockUserManager.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task WhenSuccessful_ShouldCreateUserWithRoles_And_ReturnUnit()
    {
        // Arrange
        var request = new SignUpCommand("test@test.com", "password");

        _mockUserManager
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(default(ApplicationUserEntity));

        _mockUserManager
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUserEntity>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(
                x => x.AddToRoleAsync(It.IsAny<ApplicationUserEntity>(), UserRoles.JobSeeker)
            )
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<Unit>();

        _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
        _mockUserManager.Verify(
            x => x.CreateAsync(It.IsAny<ApplicationUserEntity>(), request.Password),
            Times.Once
        );
        _mockUserManager.Verify(
            x => x.AddToRoleAsync(It.IsAny<ApplicationUserEntity>(), UserRoles.JobSeeker),
            Times.Once
        );
    }

    [Fact]
    public async Task WithExistingUser_ShouldReturnUserAlreadyExistsError()
    {
        // Arrange
        var request = new SignUpCommand("existing@test.com", "password");

        var existingUser = new ApplicationUserEntity { Id = Guid.NewGuid(), Email = request.Email };

        _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(existingUser);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AuthErrors.UserAlreadyExists);

        _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
        _mockUserManager.Verify(
            x => x.CreateAsync(It.IsAny<ApplicationUserEntity>(), request.Password),
            Times.Never
        );
    }
}