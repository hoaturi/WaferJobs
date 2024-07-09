using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Lookup.GetActiveJobCities;

public record GetActiveJobCitiesQuery : IRequest<Result<GetActiveJobCitiesResponse, Error>>;