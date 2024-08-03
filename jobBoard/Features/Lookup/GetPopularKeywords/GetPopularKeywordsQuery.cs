using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Lookup.GetPopularKeywords;

public record GetPopularKeywordsQuery : IRequest<Result<GetPopularKeywordsResponse, Error>>;