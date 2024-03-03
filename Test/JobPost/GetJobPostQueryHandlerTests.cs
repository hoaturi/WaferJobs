using FluentAssertions;
using JobBoard;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Test;

public class GetJobPostQueryHandlerTests
{
    private readonly AppDbContext _appDbContext;
    private readonly GetJobPostQueryHandler _getJobPostQueryHandler;

    public GetJobPostQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "testDb")
            .Options;

        _appDbContext = new AppDbContext(options);

        _getJobPostQueryHandler = new GetJobPostQueryHandler(_appDbContext);
    }

    [Fact]
    public async Task WhenSuccessful_ReturnJobPostResponse()
    {
        // Arrange
        var request = new GetJobPostQuery(Guid.NewGuid());

        var category = new Category
        {
            Id = 1,
            Name = "Test Category",
            Slug = "test-category"
        };
        var country = new Country
        {
            Id = 1,
            Name = "Test Country",
            Code = "TC"
        };
        var employmentType = new EmploymentType { Id = 1, Name = "Test Employment Type", };

        var jobPost = new JobPost
        {
            Id = request.Id,
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
            IsPublished = true,
            PublishedAt = DateTime.UtcNow,
        };
        _appDbContext.Categories.Add(category);
        _appDbContext.Countries.Add(country);
        _appDbContext.EmploymentTypes.Add(employmentType);
        _appDbContext.JobPosts.Add(jobPost);
        await _appDbContext.SaveChangesAsync();

        // Act
        var result = await _getJobPostQueryHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<GetJobPostResponse>();

        var response = result.Value;
        response.Should().NotBeNull();
        response.Id.Should().Be(jobPost.Id);
    }

    [Fact]
    public async Task WhenJobPostNotFound_ReturnJobPostNotFoundError()
    {
        // Arrange
        var request = new GetJobPostQuery(Guid.NewGuid());

        // Act
        var result = await _getJobPostQueryHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(JobPostErrors.JobPostNotFound(request.Id));
    }
}
