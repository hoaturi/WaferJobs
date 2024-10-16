using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Business.UpdateMyBusiness;

public record UpdateMyBusinessCommand(
    string Name,
    int? BusinessSizeId,
    string? Description,
    string? Location,
    string WebsiteUrl,
    string? TwitterUrl,
    string? LinkedInUrl
) : IRequest<Result<Unit, Error>>;