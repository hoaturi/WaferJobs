using Hangfire;
using JobBoard.Common.Models;
using JobBoard.Domain.Business.Exceptions;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.EmailService;
using JobBoard.Infrastructure.Services.EmailService.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Admin.Business.VerifyBusiness;

public class VerifyBusinessCommandHandler(
    AppDbContext dbContext,
    ILogger<VerifyBusinessCommandHandler> logger,
    IBackgroundJobClient backgroundJobClient
)
    : IRequestHandler<VerifyBusinessCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(VerifyBusinessCommand command, CancellationToken cancellationToken)
    {
        var business =
            await dbContext.Businesses
                .FirstOrDefaultAsync(
                    b => b.Id == command.BusinessId && !b.IsActive && !b.IsClaimed, cancellationToken) ??
            throw new BusinessNotFoundException(command.BusinessId);

        var member =
            await dbContext.BusinessMemberships.Include(m => m.User)
                .Where(m => m.BusinessId == business.Id && m.IsAdmin && !m.IsActive)
                .FirstOrDefaultAsync(cancellationToken) ?? throw new BusinessMembershipNotFoundException(business.Id);

        if (command.IsApproved)
        {
            business.IsActive = true;
            business.IsClaimed = true;
            member.IsActive = true;
        }
        else
        {
            dbContext.BusinessMemberships.Remove(member);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var emailDto =
            new BusinessReviewResultEmailDto(business.Id, member.User.Id, member.User.Email!, business.Name,
                command.IsApproved);
        backgroundJobClient.Enqueue<IEmailService>(x => x.SendBusinessReviewResultAsync(emailDto));

        logger.LogInformation("Completed verification for business: {BusinessId}: {Result}",
            business.Id,
            command.IsApproved ? "Approved" : "Rejected");

        return Unit.Value;
    }
}