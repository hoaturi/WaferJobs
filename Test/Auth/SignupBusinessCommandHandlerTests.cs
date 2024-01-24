using FluentAssertions;
using JobBoard;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Test;

public class SignupBusinessCommandHandlerTests
{
    private readonly Mock<IUserStore<ApplicationUser>> _mockUserStore;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly AppDbContext _appDbContext;
    private readonly SignUpBusinessCommandHandler _handler;

    public SignupBusinessCommandHandlerTests()
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

        var option = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "Test")
            .Options;
        _appDbContext = new AppDbContext(option);

        _handler = new SignUpBusinessCommandHandler(_mockUserManager.Object, _appDbContext);
    }

    [Fact]
    public async Task WhenSuccessful_ShouldCreateUser_And_Business_And_ReturnUnit()
    {
        // Arrange
        var request = new SignUpBusinessCommand("business@test.com", "password", "test Business");

        _mockUserManager
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(default(ApplicationUser));
        _mockUserManager
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(
                x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), RoleTypes.Business.ToString())
            )
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);

        _mockUserManager.Verify(x => x.FindByEmailAsync(request.Email), Times.Once);
        _mockUserManager.Verify(
            x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password),
            Times.Once
        );
        _mockUserManager.Verify(
            x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), RoleTypes.Business.ToString()),
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

        var user = new ApplicationUser { Email = request.Email, UserName = request.Email, };

        _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(AuthErrors.UserAlreadyExists);
    }
}
