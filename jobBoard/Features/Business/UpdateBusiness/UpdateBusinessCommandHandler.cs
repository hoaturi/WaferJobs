using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard;

public class UpdateBusinessCommandHandler(
    AppDbContext appDbContext,
    ICurrentUserService currentUser
) : IRequestHandler<UpdateBusinessCommand, Result<Unit, Error>>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ICurrentUserService _currentUser = currentUser;

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
            return BusinessErrors.AssociatedBusinessNotFound(userId);
        }

        UpdateBusinessMapper.MapToEntity(request, business);

        await _appDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
