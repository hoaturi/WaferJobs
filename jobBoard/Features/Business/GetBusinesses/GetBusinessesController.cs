using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.GetBusinesses;

[Tags("Business")]
[Route("api/businesses")]
[ApiController]
public class GetBusinessesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBusinesses()
    {
        var result = await sender.Send(new GetBusinessesQuery());

        return result.IsSuccess
            ? Ok(result.Value)
            : this.HandleError(result.Error);
    }
}