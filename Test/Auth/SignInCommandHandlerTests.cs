using FluentAssertions;
using JobBoard;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test;

public class SignInCommandHandlerTests
{
    private readonly Mock<IUserStore<ApplicationUser>> _mockUserStore;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<ILogger<SignInCommandHandler>> _mockLogger;
    private readonly SignInCommandHandler _handler;

    public SignInCommandHandlerTests()
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
        _mockJwtService = new Mock<IJwtService>();
        _mockLogger = new Mock<ILogger<SignInCommandHandler>>();

        _handler = new SignInCommandHandler(
            _mockUserManager.Object,
            _mockJwtService.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task WhenSuccessful_ShouldReturnSignInResponse()
    {
        // Arrange
        var request = new SignInCommand("test@test.com", "password");
        var mockUser = new ApplicationUser { Id = Guid.NewGuid(), Email = "test@test.com", };
        var mockRoles = new List<string> { RoleTypes.JobSeeker.ToString() };
        var mockAccessToken = "mockAccessToken";
        var mockRefreshToken = "mockRefreshToken";

        _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(mockUser);

        _mockUserManager
            .Setup(x => x.CheckPasswordAsync(mockUser, request.Password))
            .ReturnsAsync(true);

        _mockUserManager.Setup(x => x.GetRolesAsync(mockUser)).ReturnsAsync(mockRoles);

        _mockJwtService
            .Setup(x => x.GenerateTokens(mockUser, mockRoles))
            .Returns((mockAccessToken, mockRefreshToken));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<SignInResponse>();

        var response = result.Value;

        response.User.Id.Should().Be(mockUser.Id);
        response.User.Email.Should().Be(mockUser.Email);
        response.User.Roles.Should().BeEquivalentTo(mockRoles);
        response.AccessToken.Should().Be(mockAccessToken);
        response.RefreshToken.Should().Be(mockRefreshToken);

        _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
        _mockUserManager.Verify(x => x.CheckPasswordAsync(mockUser, request.Password), Times.Once);
        _mockUserManager.Verify(x => x.GetRolesAsync(mockUser), Times.Once);
        _mockJwtService.Verify(x => x.GenerateTokens(mockUser, mockRoles), Times.Once);
    }

    [Fact]
    public async Task WhenUserNotFound_ShouldReturnInvalidCredentialsError()
    {
        // Arrange
        var request = new SignInCommand("notExisting@test.com", "password");

        _mockUserManager
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(default(ApplicationUser));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AuthErrors.InvalidCredentials);

        _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
        _mockUserManager.Verify(
            x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()),
            Times.Never
        );
    }

    [Fact]
    public async Task WithInvalidCredentials_ShouldReturnInvalidCredentialsError()
    {
        // Arrange
        var request = new SignInCommand("test@test.com", "wrongPassword");
        var mockUser = new ApplicationUser { Id = Guid.NewGuid(), Email = request.Email };

        _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(mockUser);
        _mockUserManager
            .Setup(x => x.CheckPasswordAsync(mockUser, request.Password))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AuthErrors.InvalidCredentials);

        _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
        _mockUserManager.Verify(x => x.CheckPasswordAsync(mockUser, request.Password), Times.Once);
        _mockUserManager.Verify(x => x.GetRolesAsync(mockUser), Times.Never);
    }
}
