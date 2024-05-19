using FluentAssertions;
using JobBoard.Common.Interfaces;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Business;
using JobBoard.Features.Business.UploadMyBusinessLogo;
using JobBoard.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Test;

public class UploadBusinessLogoCommandHandlerTests
{
    private readonly AppDbContext _appDbContext;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IFileUploadService> _fileUploadServiceMock;
    private readonly UploadMyBusinessLogoCommandHandler _handler;

    public UploadBusinessLogoCommandHandlerTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _fileUploadServiceMock = new Mock<IFileUploadService>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        _appDbContext = new AppDbContext(options);

        _handler = new UploadMyBusinessLogoCommandHandler(
            _appDbContext,
            _currentUserServiceMock.Object,
            _fileUploadServiceMock.Object
        );
    }

    public static IFormFile CreateMockImageFile()
    {
        var imageData = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82 }; // PNG header bytes

        var memoryStream = new MemoryStream(imageData);

        var file = new FormFile(memoryStream, 0, memoryStream.Length, "Data", "test.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };

        return file;
    }

    [Fact]
    public async Task WhenSuccessful_ShouldUploadLogo_andUpdateBusiness()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var business = new BusinessEntity
        {
            Name = "Test Business",
            BusinessSizeId = 5,
            Description = "Test Description",
            Location = "Test Location",
            Url = "https://test.com",
            TwitterUrl = "https://twitter.com/test",
            LinkedInUrl = "https://linkedin.com/test",
            UserId = userId
        };
        var logoUrl = "https://test.com/test.jpg";

        var mockFile = CreateMockImageFile();
        var request = new UploadMyBusinessLogoCommand(mockFile);

        _appDbContext.Businesses.Add(business);
        _appDbContext.SaveChanges();

        _currentUserServiceMock.Setup(m => m.GetUserId()).Returns(userId);
        _fileUploadServiceMock
            .Setup(m => m.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync(logoUrl);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert

        result.IsSuccess.Should().BeTrue();

        var updatedBusiness = await _appDbContext.Businesses.FirstOrDefaultAsync(
            b => b.UserId == userId
        );

        updatedBusiness.Should().NotBeNull();
        updatedBusiness!.LogoUrl.Should().Be(logoUrl);
    }

    [Fact]
    public async Task WhenBusinessNotFound_ShouldThrowAssociatedBusinessNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var mockFile = CreateMockImageFile();
        var request = new UploadMyBusinessLogoCommand(mockFile);

        _currentUserServiceMock.Setup(m => m.GetUserId()).Returns(userId);

        // Act
        var act = new Func<Task>(
            async () => await _handler.Handle(request, CancellationToken.None)
        );

        // Assert
        await act.Should().ThrowAsync<BusinessNotFoundForUserException>();
    }
}