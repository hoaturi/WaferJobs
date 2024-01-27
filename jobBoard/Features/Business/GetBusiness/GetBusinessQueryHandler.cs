using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard;

public class GetBusinessQueryHandler(
    AppDbContext appDbContext,
    ILogger<GetBusinessQueryHandler> logger
) : IRequestHandler<GetBusinessQuery, Result<GetBusinessResponse, Error>>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ILogger<GetBusinessQueryHandler> _logger = logger;

    public async Task<Result<GetBusinessResponse, Error>> Handle(
        GetBusinessQuery request,
        CancellationToken cancellationToken
    )
    {
        var business = await _appDbContext
            .Businesses.AsNoTracking()
            .Where(b => b.Id == request.Id)
            .Include(b => b.BusinessSize)
            .Select(b => GetBusinessQueryMapper.MapToResponse(b))
            .FirstOrDefaultAsync(cancellationToken);

        if (business is null)
        {
            _logger.LogWarning("Business with id {businessId} not found", request.Id);
            return BusinessErrors.BusinessNotFound(request.Id);
        }

        return business;
    }
}
