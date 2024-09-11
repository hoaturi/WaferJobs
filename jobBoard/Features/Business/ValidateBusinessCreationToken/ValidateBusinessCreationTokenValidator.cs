using FluentValidation;

namespace JobBoard.Features.Business.ValidateBusinessCreationToken;

public class ValidateBusinessCreationTokenValidator : AbstractValidator<ValidateBusinessCreationTokenQuery>
{
    public ValidateBusinessCreationTokenValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required");
    }
}