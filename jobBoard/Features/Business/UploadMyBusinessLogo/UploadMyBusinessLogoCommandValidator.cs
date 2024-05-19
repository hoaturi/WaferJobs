using FluentValidation;

namespace JobBoard.Features.Business.UploadMyBusinessLogo;

public class UploadBusinessLogoCommandValidator : AbstractValidator<UploadMyBusinessLogoCommand>
{
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };

    public UploadBusinessLogoCommandValidator()
    {
        RuleFor(command => command.File)
            .NotNull()
            .WithMessage("File is required.")
            .Must(file => file.Length > 0)
            .WithMessage("File is empty.")
            .Must(IsAllowedExtension)
            .WithMessage("File must be a valid image.");
    }

    private bool IsAllowedExtension(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return _allowedExtensions.Contains(extension);
    }
}