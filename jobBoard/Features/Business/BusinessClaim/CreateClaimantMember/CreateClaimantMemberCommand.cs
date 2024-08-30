using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.BusinessClaim.CreateClaimantMember;

public record CreateClaimantMemberCommand(
    string FirstName,
    string LastName,
    string Title
) : IRequest<Result<Unit, Error>>;