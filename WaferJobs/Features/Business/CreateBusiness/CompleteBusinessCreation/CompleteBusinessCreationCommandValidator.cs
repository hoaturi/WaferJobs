using FluentValidation;

namespace WaferJobs.Features.Business.CreateBusiness.CompleteBusinessCreation;

public class CompleteBusinessCreationCommandValidator : AbstractValidator<CompleteBusinessCreationCommand>
{
    public CompleteBusinessCreationCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();

        RuleFor(x => x.Dto.FirstName).NotEmpty().MaximumLength(50);

        RuleFor(x => x.Dto.LastName).NotEmpty().MaximumLength(50);

        RuleFor(x => x.Dto.Title).NotEmpty().MaximumLength(100);
    }
}