using System.Text;
using FluentAssertions;
using JobBoard;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test;

public class SignOutCommandHandlerTests
{
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<IDistributedCache> _mockCache;
    private readonly Mock<ILogger<SignOutCommandHandler>> _mockLogger;
    private readonly SignOutCommandHandler _handler;

    public SignOutCommandHandlerTests()
    {
        _mockJwtService = new Mock<IJwtService>();
        _mockCache = new Mock<IDistributedCache>();
        _mockLogger = new Mock<ILogger<SignOutCommandHandler>>();

        _handler = new SignOutCommandHandler(
            _mockJwtService.Object,
            _mockCache.Object,
            _mockLogger.Object
        );
    }

    private static byte[] RevokedStringToBytes()
    {
        return Encoding.UTF8.GetBytes(RefreshTokenStatus.Revoked.ToString());
    }

    private static byte[] StringToBytes(string stringToConvert)
    {
        return Encoding.UTF8.GetBytes(stringToConvert);
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

        _mockCache
            .Setup(c => c.GetAsync(key, CancellationToken.None))
            .ReturnsAsync(default(byte[]));
        _mockJwtService.Setup(j => j.ValidateRefreshToken(refreshToken)).ReturnsAsync(true);
        _mockJwtService.Setup(j => j.GetExpiration(refreshToken)).Returns(expiration);
        _mockCache
            .Setup(
                c =>
                    c.SetAsync(
                        key,
                        StringToBytes(RefreshTokenStatus.Revoked.ToString()),
                        It.IsAny<DistributedCacheEntryOptions>(),
                        CancellationToken.None
                    )
            )
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockCache.Verify(c => c.GetAsync(key, CancellationToken.None), Times.Once);
        _mockJwtService.Verify(j => j.ValidateRefreshToken(refreshToken), Times.Once);
        _mockJwtService.Verify(j => j.GetExpiration(refreshToken), Times.Once);
        _mockCache.Verify(
            c =>
                c.SetAsync(
                    key,
                    RevokedStringToBytes(),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    CancellationToken.None
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task WithBlacklistedToken_ShouldReturnError_And_LogWarning()
    {
        // Arrange
        var blacklistedRefreshToken = "blacklistedToken";
        var bearerToken = $"Bearer {blacklistedRefreshToken}";
        var command = new SignOutCommand(bearerToken);
        var key = CacheKeys.RevokedToken + blacklistedRefreshToken;

        _mockCache
            .Setup(c => c.GetAsync(key, CancellationToken.None))
            .ReturnsAsync(StringToBytes(blacklistedRefreshToken));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockCache.Verify(c => c.GetAsync(key, It.IsAny<CancellationToken>()), Times.Once);
        _mockJwtService.Verify(j => j.ValidateRefreshToken(blacklistedRefreshToken), Times.Never);
        _mockJwtService.Verify(j => j.GetExpiration(blacklistedRefreshToken), Times.Never);
        _mockCache.Verify(
            c =>
                c.SetAsync(
                    key,
                    RevokedStringToBytes(),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    CancellationToken.None
                ),
            Times.Never
        );
    }

    [Fact]
    public async Task WithInvalidRefreshToken_ShouldReturnInvalidRefreshTokenError()
    {
        // Arrange
        var refreshToken = "invalidRefreshToken";
        var bearerToken = $"Bearer {refreshToken}";
        var command = new SignOutCommand(bearerToken);
        var key = CacheKeys.RevokedToken + refreshToken;

        _mockCache
            .Setup(c => c.GetAsync(key, CancellationToken.None))
            .ReturnsAsync(default(byte[]));
        _mockJwtService.Setup(j => j.ValidateRefreshToken(refreshToken)).ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockCache.Verify(c => c.GetAsync(key, CancellationToken.None), Times.Once);
        _mockJwtService.Verify(j => j.ValidateRefreshToken(refreshToken), Times.Once);
        _mockJwtService.Verify(j => j.GetExpiration(refreshToken), Times.Never);
        _mockCache.Verify(
            c =>
                c.SetAsync(
                    key,
                    RevokedStringToBytes(),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    CancellationToken.None
                ),
            Times.Never
        );
    }
}
