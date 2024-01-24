using FluentAssertions;
using JobBoard;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Xunit;

namespace Test;

public class CreateBusinessCommandHandlerTests
{
    private readonly AppDbContext _appDbContext;
    private readonly Mock<ICurrentUserService> _mockCurrentUser;
    private readonly CreateBusinessCommandHandler _handler;

    public CreateBusinessCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _appDbContext = new AppDbContext(options);

        _mockCurrentUser = new Mock<ICurrentUserService>();

        _handler = new CreateBusinessCommandHandler(_appDbContext, _mockCurrentUser.Object);
    }

    [Fact]
    public async Task WhenSuccessful_ShouldCreateBusiness_And_ReturnUnit()
    {
        // Arrange
        var request = new CreateBusinessCommand(
            "Test Business",
            5,
            "Test Description",
            "Test Location",
            "https://test.com",
            "https://twitter.com/test",
            "https://linkedin.com/test"
        );
        var user = new ApplicationUser { Id = Guid.NewGuid() };

        _appDbContext.Users.Add(user);
        await _appDbContext.SaveChangesAsync();

        _mockCurrentUser.Setup(x => x.GetUserId()).Returns(user.Id);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var createdBusiness = await _appDbContext.Businesses.FirstOrDefaultAsync(
            b => b.UserId == user.Id
        );

        createdBusiness.Should().NotBeNull();
        createdBusiness.Name.Should().Be(request.Name);
        createdBusiness.Description.Should().Be(request.Description);
        createdBusiness.Location.Should().Be(request.Location);
        createdBusiness.Size.Should().Be(BusinessSizeMapper.MapToString(request.Size));
        createdBusiness.Url.Should().Be(request.Url);
        createdBusiness.TwitterUrl.Should().Be(request.TwitterUrl);
        createdBusiness.LinkedInUrl.Should().Be(request.LinkedInUrl);
        createdBusiness.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task WhenBusinessAlreadyExists_ShouldReturnBusinessAlreadyExistsError()
    {
        // Arrange
        var request = new CreateBusinessCommand(
            "Test Business",
            5,
            "Test Description",
            "Test Location",
            "https://test.com",
            "https://twitter.com/test",
            "https://linkedin.com/test"
        );
        var user = new ApplicationUser { Id = Guid.NewGuid() };

        var business = new Business
        {
            Name = request.Name,
            Description = request.Description,
            Location = request.Location,
            Size = BusinessSizeMapper.MapToString(request.Size),
            Url = request.Url,
            TwitterUrl = request.TwitterUrl,
            LinkedInUrl = request.LinkedInUrl,
            UserId = user.Id
        };

        _appDbContext.Users.Add(user);
        _appDbContext.Businesses.Add(business);
        await _appDbContext.SaveChangesAsync();

        _mockCurrentUser.Setup(x => x.GetUserId()).Returns(user.Id);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(BusinessErrors.BusinessAlreadyExists(user.Id));
    }
}
