using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard;

public class GetMyBusinessQueryHandler(
    AppDbContext appDbContext,
    ICurrentUserService currentUserService
) : IRequestHandler<GetMyBusinessQuery, Result<GetBusinessResponse, Error>>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<Result<GetBusinessResponse, Error>> Handle(
        GetMyBusinessQuery request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = _currentUserService.GetUserId();

        var business =
            await _appDbContext
                .Businesses.Where(b => b.UserId == currentUserId)
                .Include(b => b.BusinessSize)
                .Select(b => GetBusinessQueryMapper.MapToResponse(b))
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new AssociatedBusinessNotFoundException(currentUserId);

        return business;
    }
}
