using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Lookup.GetActiveJobCountries;

public class GetActiveJobCountriesQuery : IRequest<Result<GetActiveJobCountriesResponse, Error>>;