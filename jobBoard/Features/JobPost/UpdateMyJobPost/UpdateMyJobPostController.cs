using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.JobPost.UpdateMyJobPost;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class UpdateMyJobPostController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpPut("me/{id:guid}")]
    public async Task<IActionResult> UpdateMyJobPost([FromRoute] Guid id, [FromBody] UpdateMyJobPostDto dto)
    {
        var command = new UpdateMyJobPostCommand(
            id,
            dto.CategoryId,
            dto.CountryId,
            dto.EmploymentTypeId,
            dto.Description,
            dto.Title,
            dto.CompanyName,
            dto.ApplyUrl,
            dto.IsRemote,
            dto.CompanyLogoUrl,
            dto.CompanyWebsiteUrl,
            dto.City,
            dto.MinSalary,
            dto.MaxSalary,
            dto.Currency,
            dto.Tags
        );

        var result = await sender.Send(command);

        return result.IsSuccess ? Ok() : this.HandleError(result.Error);
    }
}