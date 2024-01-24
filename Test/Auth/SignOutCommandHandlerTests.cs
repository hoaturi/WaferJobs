using System.Text;
using FluentAssertions;
using JobBoard;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace Test;

public class SignOutCommandHandlerTests
{
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<IDistributedCache> _mockCache;
    private readonly SignOutCommandHandler _handler;

    public SignOutCommandHandlerTests()
    {
        _mockJwtService = new Mock<IJwtService>();
        _mockCache = new Mock<IDistributedCache>();

        _handler = new SignOutCommandHandler(_mockJwtService.Object, _mockCache.Object);
    }

    private static byte[] RevokedStringToBytes()
    {
        return Encoding.UTF8.GetBytes(RefreshTokenStatus.Revoked.ToString());
    }

    // Verify SetAsync instead of SetStringAsync because setStringAsync is extension method
    // that convert string into byte array and then call SetAsync
    [Fact]
    public async Task WhenSuccessful_ShouldAddBlackList_And_ReturnUnit()
    {
        // Arrange
        var refreshToken = "refreshToken";
        var bearerToken = $"Bearer {refreshToken}";
        var command = new SignOutCommand(bearerToken);
        var expiration = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();
        var key = CacheKeys.RevokedToken + refreshToken;

        _mockJwtService.Setup(j => j.ValidateRefreshToken(refreshToken)).ReturnsAsync(true);
        _mockJwtService.Setup(j => j.GetExpiration(refreshToken)).Returns(expiration);
        _mockCache
            .Setup(
                c =>
                    c.SetAsync(
                        key,
                        RevokedStringToBytes(),
                        It.IsAny<DistributedCacheEntryOptions>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<Unit>();

        _mockJwtService.Verify(j => j.ValidateRefreshToken(refreshToken), Times.Once);
        _mockJwtService.Verify(j => j.GetExpiration(refreshToken), Times.Once);

        _mockCache.Verify(
            c =>
                c.SetAsync(
                    key,
                    RevokedStringToBytes(),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task WithInvalidRefreshToken_ShouldReturnInvalidRefreshTokenError()
    {
        // Arrange
        var refreshToken = "invalidRefreshToken";
        var bearerToken = $"Bearer {refreshToken}";
        var command = new SignOutCommand(bearerToken);

        _mockJwtService.Setup(j => j.ValidateRefreshToken(refreshToken)).ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AuthErrors.InvalidRefreshToken);

        _mockJwtService.Verify(j => j.ValidateRefreshToken(refreshToken), Times.Once);
        _mockJwtService.Verify(j => j.GetExpiration(refreshToken), Times.Never);
    }
}
