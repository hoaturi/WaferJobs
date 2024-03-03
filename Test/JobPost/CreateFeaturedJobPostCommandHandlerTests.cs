using FluentAssertions;
using JobBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test;

public class CreateFeaturedJobPostCommandHandlerTests
{
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly Mock<ILogger<CreateFeaturedJobPostCommandHandler>> _loggerMock;
    private readonly AppDbContext _appDbContext;
    private readonly CreateFeaturedJobPostCommandHandler _handler;

    public CreateFeaturedJobPostCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _paymentServiceMock = new Mock<IPaymentService>();
        _loggerMock = new Mock<ILogger<CreateFeaturedJobPostCommandHandler>>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "Test")
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

        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "test@test.com" };
        var business = new Business { UserId = user.Id, Name = "Test Business", };

        var customerId = "cus_123";
        var sessionId = "session_123";
        var sessionUrl = "https://stripe.com";

        _appDbContext.Users.Add(user);
        _appDbContext.Businesses.Add(business);
        await _appDbContext.SaveChangesAsync();

        _currentUserServiceMock.Setup(m => m.GetUserId()).Returns(user.Id);
        _currentUserServiceMock.Setup(m => m.GetUserEmail()).Returns(user.Email);

        _paymentServiceMock
            .Setup(m => m.CreateCustomer(business.Id, user.Email, business.Name))
            .ReturnsAsync("cus_123");
        _paymentServiceMock
            .Setup(m => m.CreateFeaturedListingCheckoutSessions(customerId))
            .ReturnsAsync(new Stripe.Checkout.Session { Id = sessionId, Url = sessionUrl });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<CreateFeaturedJobPostResponse>();
        result.Value.SessionUrl.Should().Be(sessionUrl);

        var createdBusiness = await _appDbContext.Businesses.FirstOrDefaultAsync(
            b => b.UserId == user.Id
        );
        createdBusiness.Should().NotBeNull();
        createdBusiness!.StripeCustomerId.Should().Be(customerId);

        var createdJobPost = await _appDbContext.JobPosts.FirstOrDefaultAsync(
            jp => jp.BusinessId == business.Id
        );
        createdJobPost.Should().NotBeNull();
        createdJobPost!.Title.Should().Be(command.Title);

        var createdPayment = await _appDbContext.JobPostPayments.FirstOrDefaultAsync(
            p => p.JobPostId == createdJobPost.Id
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

        var user = new ApplicationUser { Id = Guid.NewGuid() };
        _appDbContext.Users.Add(user);
        await _appDbContext.SaveChangesAsync();

        _currentUserServiceMock.Setup(m => m.GetUserId()).Returns(user.Id);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AssociatedBusinessNotFoundException>();
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

        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "" };
        var business = new Business
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
            .Setup(m => m.CreateFeaturedListingCheckoutSessions(business.StripeCustomerId))
            .ReturnsAsync(new Stripe.Checkout.Session { Id = sessionId, Url = sessionUrl });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<CreateFeaturedJobPostResponse>();
        result.Value.SessionUrl.Should().Be(sessionUrl);

        var createdBusiness = await _appDbContext.Businesses.FirstOrDefaultAsync(
            b => b.UserId == user.Id
        );
        createdBusiness.Should().NotBeNull();
        createdBusiness!.StripeCustomerId.Should().Be(business.StripeCustomerId);
    }
}
