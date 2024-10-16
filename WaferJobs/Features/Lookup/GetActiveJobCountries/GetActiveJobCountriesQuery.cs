using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Lookup.GetActiveJobCountries;

public class GetActiveJobCountriesQuery : IRequest<Result<GetActiveJobCountriesResponse, Error>>;