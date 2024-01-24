using FluentAssertions;
using JobBoard;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Test;

public class UpdateBusinessCommandTests
{
    private readonly AppDbContext _appDbContext;
    private readonly Mock<ICurrentUserService> _mockCurrentUser;
    private readonly UpdateBusinessCommandHandler _handler;

    public UpdateBusinessCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _appDbContext = new AppDbContext(options);
        _mockCurrentUser = new Mock<ICurrentUserService>();

        _handler = new UpdateBusinessCommandHandler(_appDbContext, _mockCurrentUser.Object);
    }

    [Fact]
    public async Task WhenSuccessful_ShouldUpdateBusiness_And_ReturnUnit()
    {
        // Arrange
        var user = new ApplicationUser { Id = Guid.NewGuid() };
        var business = new Business
        {
            Name = "Test Business",
            Size = "5",
            Description = "Test Description",
            Location = "Test Location",
            Url = "https://test.com",
            TwitterUrl = "https://twitter.com/test",
            LinkedInUrl = "https://linkedin.com/test",
            UserId = user.Id
        };

        var request = new UpdateBusinessCommand(
            "Updated Business",
            2,
            "Updated Description",
            "Updated Location",
            "https://updated.com",
            "https://twitter.com/updated",
            "https://linkedin.com/updated"
        );

        _appDbContext.Users.Add(user);
        _appDbContext.Businesses.Add(business);
        await _appDbContext.SaveChangesAsync();

        _mockCurrentUser.Setup(x => x.GetUserId()).Returns(user.Id);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedBusiness = await _appDbContext.Businesses.FirstOrDefaultAsync(
            b => b.UserId == user.Id
        );

        updatedBusiness.Should().NotBeNull();
        updatedBusiness.Name.Should().Be(request.Name);
        updatedBusiness.Description.Should().Be(request.Description);
        updatedBusiness.Location.Should().Be(request.Location);
        updatedBusiness.Size.Should().Be(BusinessSizeMapper.MapToString(request.Size));
        updatedBusiness.Url.Should().Be(request.Url);
        updatedBusiness.TwitterUrl.Should().Be(request.TwitterUrl);
        updatedBusiness.LinkedInUrl.Should().Be(request.LinkedInUrl);
        updatedBusiness.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task WhenBusinessNotFound_ShouldReturnAssociatedBusinessNotFound()
    {
        // Arrange
        var user = new ApplicationUser { Id = Guid.NewGuid() };

        var request = new UpdateBusinessCommand(
            "Updated Business",
            2,
            "Updated Description",
            "Updated Location",
            "https://updated.com",
            "https://twitter.com/updated",
            "https://linkedin.com/updated"
        );

        _appDbContext.Users.Add(user);
        await _appDbContext.SaveChangesAsync();

        _mockCurrentUser.Setup(x => x.GetUserId()).Returns(user.Id);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(BusinessErrors.AssociatedBusinessNotFound(user.Id));
    }
}
