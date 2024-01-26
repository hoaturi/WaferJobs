using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard;

public class UpdateBusinessCommandHandler(
    AppDbContext appDbContext,
    ICurrentUserService currentUser,
    ILogger<UpdateBusinessCommandHandler> logger
) : IRequestHandler<UpdateBusinessCommand, Result<Unit, Error>>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly ILogger<UpdateBusinessCommandHandler> _logger = logger;

    public async Task<Result<Unit, Error>> Handle(
        UpdateBusinessCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = _currentUser.GetUserId();

        var business = await _appDbContext
            .Businesses.Where(b => b.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (business is null)
        {
            _logger.LogError("Business not found for user: {UserId}", userId);
            return BusinessErrors.AssociatedBusinessNotFound(userId);
        }

        UpdateBusinessCommandMapper.MapToBusiness(request, business);

        await _appDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated business: {BusinessId}", business.Id);

        return Unit.Value;
    }
}
