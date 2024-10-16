using MediatR;
using WaferJobs.Common.Models;
using WaferJobs.Features.Business.GetBusiness;

namespace WaferJobs.Features.Business.GetMyBusiness;

public record GetMyBusinessQuery : IRequest<Result<GetBusinessResponse, Error>>;