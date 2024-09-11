using JobBoard.Common.Models;
using JobBoard.Infrastructure.Services.DomainValidationService;
using MediatR;

namespace JobBoard.Features.Lookup.ValidatePublicDomain;

public class ValidatePublicDomainCommandHandler(IDomainValidationService domainValidationService)
    : IRequestHandler<ValidatePublicDomainCommand, Result<ValidatePublicDomainResponse, Error>>
{
    public async Task<Result<ValidatePublicDomainResponse, Error>> Handle(ValidatePublicDomainCommand command,
        CancellationToken cancellationToken)
    {
        var isPublicEmailDomain = await domainValidationService.IsPublicEmailDomainAsync(command.Domain);

        return new ValidatePublicDomainResponse(isPublicEmailDomain);
    }
}