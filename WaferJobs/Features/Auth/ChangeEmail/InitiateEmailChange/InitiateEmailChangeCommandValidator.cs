﻿using FluentValidation;

namespace WaferJobs.Features.Auth.ChangeEmail.InitiateEmailChange;

public class InitiateEmailChangeCommandValidator : AbstractValidator<InitiateEmailChangeCommand>
{
    public InitiateEmailChangeCommandValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}