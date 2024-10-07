using FluentValidation;

namespace JobBoard.Features.Business.VerifyBusinessCreationToken;

public class VerifyBusinessCreationTokenValidator : AbstractValidator<VerifyBusinessCreationTokenQuery>
{
    public VerifyBusinessCreationTokenValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required");
    }
}