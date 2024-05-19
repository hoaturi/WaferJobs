using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.UpdateMyBusiness;

public record UpdateMyBusinessCommand(
    string Name,
    int? BusinessSizeId,
    string? Description,
    string? Location,
    string? Url,
    string? TwitterUrl,
    string? LinkedInUrl
) : IRequest<Result<Unit, Error>>;