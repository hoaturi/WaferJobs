using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard;

public class UpdateMyBusinessCommandHandler(
    AppDbContext appDbContext,
    ICurrentUserService currentUser,
    ILogger<UpdateMyBusinessCommandHandler> logger
) : IRequestHandler<UpdateMyBusinessCommand, Result<Unit, Error>>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly ILogger<UpdateMyBusinessCommandHandler> _logger = logger;

    public async Task<Result<Unit, Error>> Handle(
        UpdateMyBusinessCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = _currentUser.GetUserId();

        var business =
            await _appDbContext
                .Businesses.Where(b => b.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new AssociatedBusinessNotFoundException(userId);

        UpdateMyBusinessCommandMapper.MapToEntity(request, business);

        await _appDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated business: {BusinessId}", business.Id);

        return Unit.Value;
    }
}
