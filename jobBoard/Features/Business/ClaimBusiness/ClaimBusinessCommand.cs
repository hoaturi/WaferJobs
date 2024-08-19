using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.ClaimBusiness;

public record ClaimBusinessCommand(
    Guid BusinessId,
    string FirstName,
    string LastName,
    string Title
) : IRequest<Result<Unit, Error>>;