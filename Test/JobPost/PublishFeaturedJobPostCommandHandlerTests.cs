using FluentAssertions;
using JobBoard;
using JobBoard.Domain.JobPost;
using JobBoard.Features.JobPost.PublishFeaturedJobPost;
using JobBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test;

public class PublishFeaturedJobPostCommandHandlerTests : IDisposable
{
    private readonly AppDbContext _appDbContext;
    private readonly PublishFeaturedJobPostCommandHandler _handler;
    private readonly Mock<ILogger<PublishFeaturedJobPostCommandHandler>> _mockLogger;

    public PublishFeaturedJobPostCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        _appDbContext = new AppDbContext(options);
        _mockLogger = new Mock<ILogger<PublishFeaturedJobPostCommandHandler>>();

        _handler = new PublishFeaturedJobPostCommandHandler(_appDbContext, _mockLogger.Object);
    }

    public void Dispose()
    {
        _appDbContext.Database.EnsureDeleted();
        _appDbContext.Dispose();
    }

    private async Task<JobPostPaymentEntity> SetupJobPostPayment(bool isProcessed)
    {
        var sessionId = "cs_test";
        var jobPost = new JobPostEntity
        {
            CategoryId = 1,
            CountryId = 1,
            EmploymentTypeId = 1,
            Title = "Test Job Post",
            Description = "Test Job Post Description",
            CompanyName = "Test Company",
            ApplyUrl = "https://test.com",
            City = "Test City",
            MinSalary = 1000,
            MaxSalary = 2000,
            Currency = "USD",
            CompanyLogoUrl = "https://test.com/logo.png",
            IsRemote = true,
            IsFeatured = true,
            IsPublished = false
        };

        var jobPostPayment = new JobPostPaymentEntity
        {
            JobPostEntity = jobPost,
            CheckoutSessionId = sessionId,
            EventId = "evt_123",
            IsProcessed = false
        };

        _appDbContext.JobPosts.Add(jobPost);
        _appDbContext.JobPostPayments.Add(jobPostPayment);
        await _appDbContext.SaveChangesAsync();

        return jobPostPayment;
    }

    [Fact]
    public async Task WhenSuccessful_ShouldChangePublishStatus()
    {
        var jobPostPayment = await SetupJobPostPayment(false);

        var request = new PublishFeaturedJobPostCommand(
            "evt_123",
            jobPostPayment.CheckoutSessionId
        );

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        jobPostPayment.JobPostEntity.IsPublished.Should().BeTrue();
    }

    [Fact]
    public async Task WhenJobPostNotFound_ShouldThrowJobPostToPublishNotFoundException()
    {
        var request = new PublishFeaturedJobPostCommand("test_event", "non_existent_session");

        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

        await act.Should().ThrowAsync<JobPostPaymentNotFoundException>();
    }

    [Fact]
    public async Task WhenPaymentAlreadyProcessed_ShouldReturn()
    {
        var jobPostPayment = await SetupJobPostPayment(true);
        var request = new PublishFeaturedJobPostCommand(
            "evt_123",
            jobPostPayment.CheckoutSessionId
        );

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}