using FluentValidation;

namespace JobBoard;

public class UploadBusinessLogoCommandValidator : AbstractValidator<UploadMyBusinessLogoCommand>
{
    private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png"];

    public UploadBusinessLogoCommandValidator()
    {
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("File is required.")
            .Must(x => x.Length > 0)
            .WithMessage("File is empty.")
            .Must(x =>
            {
                var extension = Path.GetExtension(x.FileName);
                return _allowedExtensions.Contains(extension);
            })
            .WithMessage("File must be a valid image.");
    }
}
