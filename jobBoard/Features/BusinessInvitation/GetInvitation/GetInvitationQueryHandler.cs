using JobBoard.Common.Models;
using JobBoard.Domain.Business.Exceptions;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.BusinessInvitation.GetInvitation;

public class GetInvitationQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetInvitationQuery, Result<GetInvitationResponse, Error>>
{
    public async Task<Result<GetInvitationResponse, Error>> Handle(GetInvitationQuery query,
        CancellationToken cancellationToken)
    {
        var invitation = await dbContext.BusinessMemberInvitations.Where(bmi => bmi.Token == query.Token)
            .Select(bmi => new { InviterEmail = bmi.Inviter.User.Email, bmi.InviteeEmail })
            .FirstOrDefaultAsync(cancellationToken);

        if (invitation is null) throw new InvitationNotFoundException(query.Token);

        return new GetInvitationResponse(invitation.InviterEmail!, invitation.InviteeEmail);
    }
}