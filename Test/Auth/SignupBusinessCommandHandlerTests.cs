using JobBoard.Common.Constants;
using JobBoard.Domain.Auth;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test;

public class SignupBusinessCommandHandlerTests
{
    private readonly AppDbContext _appDbContext;
    private readonly SignUpBusinessCommandHandler _handler;
    private readonly Mock<ILogger<SignUpBusinessCommandHandler>> _mockLogger;
    private readonly Mock<UserManager<ApplicationUserEntity>> _mockUserManager;
    private readonly Mock<IUserStore<ApplicationUserEntity>> _mockUserStore;

    public SignupBusinessCommandHandlerTests()
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
        _mockLogger = new Mock<ILogger<SignUpBusinessCommandHandler>>();

        var option = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("Test")
            .Options;
        _appDbContext = new AppDbContext(option);

        _handler = new SignUpBusinessCommandHandler(
            _mockUserManager.Object,
            _appDbContext,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task WhenSuccessful_ShouldCreateUser_And_Business_And_ReturnUnit()
    {
        // Arrange
        var request = new SignUpBusinessCommand("business@test.com", "password", "test Business");

        _mockUserManager
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(default(ApplicationUserEntity));
        _mockUserManager
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUserEntity>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(
                x => x.AddToRoleAsync(It.IsAny<ApplicationUserEntity>(), UserRoles.Business)
            )
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);

        _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
        _mockUserManager.Verify(
            x => x.CreateAsync(It.IsAny<ApplicationUserEntity>(), request.Password),
            Times.Once
        );
        _mockUserManager.Verify(
            x => x.AddToRoleAsync(It.IsAny<ApplicationUserEntity>(), UserRoles.Business),
            Times.Once
        );

        var createdBusiness = await _appDbContext
            .Businesses.Where(b => b.Name == request.CompanyName)
            .FirstOrDefaultAsync();

        createdBusiness.Should().NotBeNull();
    }

    [Fact]
    public async Task WhenEmailIsInUse_ShouldReturnUserAlreadyExistsError()
    {
        // Arrange
        var request = new SignUpBusinessCommand("business@test.com", "password", "test Business");

        var user = new ApplicationUserEntity { Email = request.Email, UserName = request.Email };

        _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AuthErrors.UserAlreadyExists);
    }
}