using MediatR;

namespace JobBoard;

public class UploadBusinessLogoCommandHandler(
    AppDbContext appDbContext,
    ICurrentUserService currentUserService,
    IFileUploadService fileUploadService
) : IRequestHandler<UploadBusinessLogoCommand, Result<Unit, Error>>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IFileUploadService _fileUploadService = fileUploadService;

    public async Task<Result<Unit, Error>> Handle(
        UploadBusinessLogoCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = _currentUserService.GetUserId();

        var business =
            _appDbContext.Businesses.Where(b => b.UserId == userId).FirstOrDefault()
            ?? throw new AssociatedBusinessNotFoundException(userId);

        var originalFileExtension = Path.GetExtension(request.File.FileName);

        var fileName = $"{business.Id}{originalFileExtension}";

        var logoUrl = await _fileUploadService.UploadBusinessLogoAsync(
            fileName,
            request.File.OpenReadStream()
        );

        business.LogoUrl = logoUrl;

        await _appDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
