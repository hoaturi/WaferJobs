using FluentAssertions;
using JobBoard.Common.Interfaces;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Business;
using JobBoard.Features.Business.GetBusiness;
using JobBoard.Features.Business.GetMyBusiness;
using JobBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Test;

public class GetMyBusinessQueryHandlerTests
{
    private readonly AppDbContext _appDbContext;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly GetMyBusinessQueryHandler _getMyBusinessQueryHandler;

    public GetMyBusinessQueryHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("Test")
            .Options;
        _appDbContext = new AppDbContext(options);

        _getMyBusinessQueryHandler = new GetMyBusinessQueryHandler(
            _appDbContext,
            _currentUserServiceMock.Object
        );
    }

    [Fact]
    public async Task WhenSuccessful_ReturnBusinessResponse()
    {
        // Arrange
        var user = new ApplicationUserEntity { Id = Guid.NewGuid() };
        var business = new BusinessEntity { UserId = user.Id, Name = "Test Business" };

        _appDbContext.Users.Add(user);
        _appDbContext.Businesses.Add(business);
        await _appDbContext.SaveChangesAsync();

        _currentUserServiceMock.Setup(m => m.GetUserId()).Returns(user.Id);

        // Act
        var result = await _getMyBusinessQueryHandler.Handle(
            new GetMyBusinessQuery(),
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.Should().BeTrue();

        var response = result.Value;
        response.Should().NotBeNull();
        response.Should().BeOfType<GetBusinessResponse>();
        response.Name.Should().Be(business.Name);
    }

    [Fact]
    public async Task WhenBusinessNotFound_ThrowAssociatedBusinessNotFoundException()
    {
        // Arrange
        var user = new ApplicationUserEntity { Id = Guid.NewGuid() };

        _appDbContext.Users.Add(user);
        await _appDbContext.SaveChangesAsync();

        _currentUserServiceMock.Setup(m => m.GetUserId()).Returns(user.Id);

        // Act
        Func<Task> act = async () =>
            await _getMyBusinessQueryHandler.Handle(
                new GetMyBusinessQuery(),
                CancellationToken.None
            );

        // Assert
        await act.Should().ThrowAsync<BusinessNotFoundForUserException>();
    }
}