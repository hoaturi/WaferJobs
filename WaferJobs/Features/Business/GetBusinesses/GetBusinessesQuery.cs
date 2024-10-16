using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Business.GetBusinesses;

public record GetBusinessesQuery : IRequest<Result<GetBusinessesResponse, Error>>;