using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Lookup.GetPopularKeywords;

public record GetPopularKeywordsQuery : IRequest<Result<GetPopularKeywordsResponse, Error>>;