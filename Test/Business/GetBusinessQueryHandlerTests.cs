using FluentAssertions;
using JobBoard;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Test;

public class GetBusinessQueryHandlerTests
{
    private readonly AppDbContext _appDbContext;
    private readonly GetBusinessQueryHandler _handler;

    public GetBusinessQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _appDbContext = new AppDbContext(options);

        _handler = new GetBusinessQueryHandler(_appDbContext);
    }

    [Fact]
    public async Task WhenSuccessful_ReturnBusinessResponse()
    {
        // Arrange
        var request = new GetBusinessQuery(Guid.NewGuid());
        var user = new ApplicationUser { Id = Guid.NewGuid() };
        var business = new Business
        {
            Id = request.Id,
            LogoUrl = "https://test.com/logo.png",
            Name = "Test Business",
            BusinessSizeId = 5,
            Description = "Test Description",
            Location = "Test Location",
            Url = "https://test.com",
            TwitterUrl = "https://twitter.com/test",
            LinkedInUrl = "https://linkedin.com/test",
            UserId = user.Id
        };

        _appDbContext.Users.Add(user);
        _appDbContext.Businesses.Add(business);
        await _appDbContext.SaveChangesAsync();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var response = result.Value;
        response.Should().NotBeNull();
        response.Should().BeOfType<GetBusinessResponse>();
        response.Id.Should().Be(business.Id);
        response.Name.Should().Be(business.Name);
        response.LogoUrl.Should().Be(business.LogoUrl);
        response.Size.Should().Be(business.BusinessSize);
        response.Description.Should().Be(business.Description);
        response.Location.Should().Be(business.Location);
        response.Url.Should().Be(business.Url);
        response.TwitterUrl.Should().Be(business.TwitterUrl);
        response.LinkedInUrl.Should().Be(business.LinkedInUrl);
    }

    [Fact]
    public async Task WhenBusinessNotFound_ReturnBusinessNotFoundError()
    {
        // Arrange
        var request = new GetBusinessQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(BusinessErrors.BusinessNotFound(request.Id));
    }
}
