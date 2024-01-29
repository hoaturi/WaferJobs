using FluentAssertions;
using JobBoard;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test;

public class SignupCommandHandlerTests
{
    private readonly Mock<IUserStore<ApplicationUser>> _mockUserStore;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<ILogger<SignUpCommandHandler>> _mockLogger;
    private readonly SignUpCommandHandler _handler;

    public SignupCommandHandlerTests()
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
            .ReturnsAsync(default(ApplicationUser));

        _mockUserManager
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(
                x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), RoleTypes.JobSeeker.ToString())
            )
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<Unit>();

        _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
        _mockUserManager.Verify(
            x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password),
            Times.Once
        );
        _mockUserManager.Verify(
            x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), RoleTypes.JobSeeker.ToString()),
            Times.Once
        );
    }

    [Fact]
    public async Task WithExistingUser_ShouldReturnUserAlreadyExistsError()
    {
        // Arrange
        var request = new SignUpCommand("existing@test.com", "password");

        var existingUser = new ApplicationUser { Id = Guid.NewGuid(), Email = request.Email };

        _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(existingUser);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AuthErrors.UserAlreadyExists);

        _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
        _mockUserManager.Verify(
            x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password),
            Times.Never
        );
    }
}
