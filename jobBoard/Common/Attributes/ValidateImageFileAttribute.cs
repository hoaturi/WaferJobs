using System.ComponentModel.DataAnnotations;
using FileTypeChecker.Extensions;
using FileTypeChecker.Types;

namespace JobBoard.Common.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class ValidateImageFileAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not IFormFile file) return false;

        var fileStream = file.OpenReadStream();
        return fileStream.Is<Webp>()
               || fileStream.Is<JointPhotographicExpertsGroup>()
               || fileStream.Is<GraphicsInterchangeFormat87>()
               || fileStream.Is<GraphicsInterchangeFormat89>()
               || fileStream.Is<PortableNetworkGraphic>();
    }

    public override string FormatErrorMessage(string name)
    {
        return "Invalid file type. Only images are allowed";
    }
}