using JobBoard.Common.Extensions;
using JobBoard.Features.Lookup.GetActiveJobCities;
using JobBoard.Features.Lookup.GetActiveJobCountries;
using JobBoard.Features.Lookup.GetActiveJobLocations;
using JobBoard.Features.Lookup.GetJobPostCount;
using JobBoard.Features.Lookup.GetPopularKeywords;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Lookup;

[Tags("Lookups")]
[ApiController]
[Route("api/lookups")]
public class LookupsController(ISender sender) : ControllerBase
{
    [HttpGet("active-job-countries")]
    public async Task<IActionResult> GetActiveJobCountries()
    {
        var result = await sender.Send(new GetActiveJobCountriesQuery());

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }

    [HttpGet("active-job-cities")]
    public async Task<IActionResult> GetActiveJobCities()
    {
        var result = await sender.Send(new GetActiveJobCitiesQuery());

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }

    [HttpGet("active-job-locations")]
    public async Task<IActionResult> GetActiveJobLocations()
    {
        var result = await sender.Send(new GetActiveJobLocationsQuery());

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }

    [HttpGet("popular-keywords")]
    public async Task<IActionResult> GetPopularKeywords()
    {
        var result = await sender.Send(new GetPopularKeywordsQuery());

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }

    [HttpGet("job-count")]
    public async Task<IActionResult> GetTotalJobPostCount()
    {
        var result = await sender.Send(new GetJobPostCountQuery());

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }
}