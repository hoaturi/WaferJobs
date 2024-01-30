using FluentAssertions;
using JobBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test;

public class UpdateMyBusinessCommandTests
{
    private readonly AppDbContext _appDbContext;
    private readonly Mock<ICurrentUserService> _mockCurrentUser;
    private readonly Mock<ILogger<UpdateMyBusinessCommandHandler>> _mockLogger;
    private readonly UpdateMyBusinessCommandHandler _handler;

    public UpdateMyBusinessCommandTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _appDbContext = new AppDbContext(options);
        _mockCurrentUser = new Mock<ICurrentUserService>();
        _mockLogger = new Mock<ILogger<UpdateMyBusinessCommandHandler>>();

        _handler = new UpdateMyBusinessCommandHandler(
            _appDbContext,
            _mockCurrentUser.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task WhenSuccessful_ShouldUpdateBusiness_And_ReturnUnit()
    {
        // Arrange
        var user = new ApplicationUser { Id = Guid.NewGuid() };
        var business = new Business
        {
            Name = "Test Business",
            BusinessSizeId = 5,
            Description = "Test Description",
            Location = "Test Location",
            Url = "https://test.com",
            TwitterUrl = "https://twitter.com/test",
            LinkedInUrl = "https://linkedin.com/test",
            UserId = user.Id
        };

        var request = new UpdateMyBusinessCommand(
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
        updatedBusiness!.Name.Should().Be(request.Name);
        updatedBusiness.Description.Should().Be(request.Description);
        updatedBusiness.Location.Should().Be(request.Location);
        updatedBusiness.BusinessSizeId.Should().Be(request.BusinessSizeId);
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

        var request = new UpdateMyBusinessCommand(
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
        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AssociatedBusinessNotFoundException>();
    }
}
