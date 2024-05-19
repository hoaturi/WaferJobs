using FluentAssertions;
using JobBoard.Common.Interfaces;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Business;
using JobBoard.Features.JobPost.CreateFeaturedJobPost;
using JobBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Stripe.Checkout;
using Xunit;

namespace Test;

public class CreateFeaturedJobPostCommandHandlerTests
{
    private readonly AppDbContext _appDbContext;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly CreateFeaturedJobPostCommandHandler _handler;
    private readonly Mock<ILogger<CreateFeaturedJobPostCommandHandler>> _loggerMock;
    private readonly Mock<IPaymentService> _paymentServiceMock;

    public CreateFeaturedJobPostCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _paymentServiceMock = new Mock<IPaymentService>();
        _loggerMock = new Mock<ILogger<CreateFeaturedJobPostCommandHandler>>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("Test")
            .Options;
        _appDbContext = new AppDbContext(options);

        _handler = new CreateFeaturedJobPostCommandHandler(
            _currentUserServiceMock.Object,
            _paymentServiceMock.Object,
            _appDbContext,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task WhenSuccessful_ShouldCreateJobPost_AndStripeCustomer_AndCheckoutSession_AndPaymentRecord()
    {
        // Arrange
        var command = new CreateFeaturedJobPostCommand(
            CategoryId: 1,
            CountryId: 1,
            EmploymentTypeId: 1,
            Description: "Test Description",
            Title: "Test Title",
            CompanyName: "Test Company",
            ApplyUrl: "https://test.com",
            IsRemote: true,
            City: "Test City",
            MinSalary: 1000,
            MaxSalary: 2000,
            Currency: "USD"
        );

        var user = new ApplicationUserEntity { Id = Guid.NewGuid(), Email = "test@test.com" };
        var business = new BusinessEntity { UserId = user.Id, Name = "Test Business" };

        var customerId = "cus_123";
        var sessionId = "session_123";
        var sessionUrl = "https://stripe.com";

        _appDbContext.Users.Add(user);
        _appDbContext.Businesses.Add(business);
        await _appDbContext.SaveChangesAsync();

        _currentUserServiceMock.Setup(m => m.GetUserId()).Returns(user.Id);
        _currentUserServiceMock.Setup(m => m.GetUserEmail()).Returns(user.Email);

        _paymentServiceMock
            .Setup(m => m.CreateStripeCustomer(business.Id, user.Email, business.Name))
            .ReturnsAsync("cus_123");
        _paymentServiceMock
            .Setup(m => m.CreateCheckoutSession(customerId))
            .ReturnsAsync(new Session { Id = sessionId, Url = sessionUrl });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<CreateFeaturedJobPostResponse>();
        result.Value.SessionUrl.Should().Be(sessionUrl);

        var createdBusiness = await _appDbContext.Businesses.FirstOrDefaultAsync(b =>
            b.UserId == user.Id
        );
        createdBusiness.Should().NotBeNull();
        createdBusiness!.StripeCustomerId.Should().Be(customerId);

        var createdJobPost = await _appDbContext.JobPosts.FirstOrDefaultAsync(jp =>
            jp.BusinessId == business.Id
        );
        createdJobPost.Should().NotBeNull();
        createdJobPost!.Title.Should().Be(command.Title);

        var createdPayment = await _appDbContext.JobPostPayments.FirstOrDefaultAsync(p =>
            p.JobPostId == createdJobPost.Id
        );
        createdPayment.Should().NotBeNull();
        createdPayment!.CheckoutSessionId.Should().Be(sessionId);
        createdPayment.JobPostId.Should().Be(createdJobPost.Id);
    }

    [Fact]
    public async Task WhenBusinessNotFound_ShouldThrowAssociatedBusinessNotFoundException()
    {
        // Arrange
        var command = new CreateFeaturedJobPostCommand(
            CategoryId: 1,
            CountryId: 1,
            EmploymentTypeId: 1,
            Description: "Test Description",
            Title: "Test Title",
            CompanyName: "Test Company",
            ApplyUrl: "https://test.com",
            IsRemote: true,
            City: "Test City",
            MinSalary: 1000,
            MaxSalary: 2000,
            Currency: "USD"
        );

        var user = new ApplicationUserEntity { Id = Guid.NewGuid() };
        _appDbContext.Users.Add(user);
        await _appDbContext.SaveChangesAsync();

        _currentUserServiceMock.Setup(m => m.GetUserId()).Returns(user.Id);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessNotFoundForUserException>();
    }

    [Fact]
    public async Task WhenStripeCustomerExists_ShouldNotCreateStripeCustomer()
    {
        // Arrange
        var command = new CreateFeaturedJobPostCommand(
            CategoryId: 1,
            CountryId: 1,
            EmploymentTypeId: 1,
            Description: "Test Description",
            Title: "Test Title",
            CompanyName: "Test Company",
            ApplyUrl: "https://test.com",
            IsRemote: true,
            City: "Test City",
            MinSalary: 1000,
            MaxSalary: 2000,
            Currency: "USD"
        );

        var user = new ApplicationUserEntity { Id = Guid.NewGuid(), Email = "" };
        var business = new BusinessEntity
        {
            UserId = user.Id,
            Name = "Test Business",
            StripeCustomerId = "cus_123"
        };

        var sessionId = "session_123";
        var sessionUrl = "https://stripe.com";

        _appDbContext.Users.Add(user);
        _appDbContext.Businesses.Add(business);
        await _appDbContext.SaveChangesAsync();

        _currentUserServiceMock.Setup(m => m.GetUserId()).Returns(user.Id);
        _currentUserServiceMock.Setup(m => m.GetUserEmail()).Returns(user.Email);

        _paymentServiceMock
            .Setup(m => m.CreateCheckoutSession(business.StripeCustomerId))
            .ReturnsAsync(new Session { Id = sessionId, Url = sessionUrl });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<CreateFeaturedJobPostResponse>();
        result.Value.SessionUrl.Should().Be(sessionUrl);

        var createdBusiness = await _appDbContext.Businesses.FirstOrDefaultAsync(b =>
            b.UserId == user.Id
        );
        createdBusiness.Should().NotBeNull();
        createdBusiness!.StripeCustomerId.Should().Be(business.StripeCustomerId);
    }
}