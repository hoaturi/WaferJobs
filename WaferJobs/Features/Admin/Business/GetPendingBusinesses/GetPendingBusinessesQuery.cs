using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Admin.Business.GetPendingBusinesses;

public record GetPendingBusinessesQuery : IRequest<Result<GetPendingBusinessesQueryResponse, Error>>;