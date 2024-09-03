using Hangfire;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Infrastructure.Services.EmailService;
using JobBoard.Infrastructure.Services.EmailService.Dtos;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Features.Auth.ResendConfirmation;

public class ResendConfirmationCommandHandler(
    UserManager<ApplicationUserEntity> userManager,
    IBackgroundJobClient backgroundJobClient)
    : IRequestHandler<ResendConfirmationCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(ResendConfirmationCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || user.EmailConfirmed)
            return Unit.Value;

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var dto = new ConfirmEmailDto(user.Email!, user.Id, token);
        backgroundJobClient.Enqueue<IEmailService>(x => x.SendEmailConfirmAsync(dto));

        return Unit.Value;
    }
}