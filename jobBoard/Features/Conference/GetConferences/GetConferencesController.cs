using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace JobBoard.Features.Conference.GetConferences;

[Tags("Conference")]
[Route("api/conferences")]
[ApiController]
public class GetConferencesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [OutputCache(PolicyName = nameof(OutputCacheKeys.ExpireIn5Minutes))]
    public async Task<IActionResult> GetConferences()
    {
        var result = await sender.Send(new GetConferencesQuery());

        return result.IsSuccess
            ? Ok(result.Value)
            : this.HandleError(result.Error);
    }
}