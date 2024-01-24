using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard;

public class CreateBusinessCommandHandler(
    AppDbContext appDbContext,
    ICurrentUserService currentUser
) : IRequestHandler<CreateBusinessCommand, Result<Unit, Error>>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task<Result<Unit, Error>> Handle(
        CreateBusinessCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = _currentUser.GetUserId();

        var businessExists = await _appDbContext
            .Businesses.AsNoTracking()
            .AnyAsync(b => b.UserId == userId, cancellationToken);

        if (businessExists)
        {
            return BusinessErrors.BusinessAlreadyExists(userId);
        }

        var business = CreateBusinessMapper.MapToEntity(request, userId);

        await _appDbContext.Businesses.AddAsync(business, cancellationToken);
        await _appDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
