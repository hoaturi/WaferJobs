using JobBoard.Common.Models;
using JobBoard.Infrastructure.Services.CachingServices.PopularKeywordsService;
using MediatR;

namespace JobBoard.Features.Lookup.GetPopularKeywords;

public class GetPopularKeywordsQueryHandler(IPopularKeywordsService popularKeywordsService)
    : IRequestHandler<GetPopularKeywordsQuery, Result<GetPopularKeywordsResponse, Error>>
{
    public async Task<Result<GetPopularKeywordsResponse, Error>> Handle(GetPopularKeywordsQuery query,
        CancellationToken cancellationToken)
    {
        var popularKeywords = await popularKeywordsService.GetPopularKeywordsAsync(cancellationToken);
        return new GetPopularKeywordsResponse(popularKeywords);
    }
}