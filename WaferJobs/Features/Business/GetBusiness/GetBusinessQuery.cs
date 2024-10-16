using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Business.GetBusiness;

public record GetBusinessQuery(string Slug) : IRequest<Result<GetBusinessResponse, Error>>;