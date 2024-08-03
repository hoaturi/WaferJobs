using JobBoard.Infrastructure.Services.LookupServices.CurrencyService;
using JobBoard.Infrastructure.Services.LookupServices.JobPostCountService;
using JobBoard.Infrastructure.Services.LookupServices.LocationService;
using JobBoard.Infrastructure.Services.LookupServices.PopularKeywordsService;

namespace JobBoard.Infrastructure.BackgroundJobs.LookupDataCacheUpdater;

public class LookupDataCacheUpdater(
    IJobPostCountService jobPostCountService,
    ILocationService locationService,
    IPopularKeywordsService popularKeywordsService,
    ILogger<LookupDataCacheUpdater> logger)
    : IRecurringJobBase
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting lookup data cache update");

        await UpdateJobPostCountAsync(cancellationToken);
        await UpdateLocationsAsync(cancellationToken);
        await UpdatePopularKeywordsAsync(cancellationToken);

        logger.LogInformation("Lookup data cache update completed successfully");
    }

    private async Task UpdateJobPostCountAsync(CancellationToken cancellationToken)
    {
        await jobPostCountService.GetJobPostCountAsync(cancellationToken);
    }

    private async Task UpdateLocationsAsync(CancellationToken cancellationToken)
    {
        await locationService.GetCountriesWithActiveJobPostAsync(cancellationToken);
        await locationService.GetCitiesWithActiveJobPostAsync(cancellationToken);
        await locationService.GetLocationsWithActiveJobPostAsync(cancellationToken);
    }

    private async Task UpdatePopularKeywordsAsync(CancellationToken cancellationToken)
    {
        await popularKeywordsService.GetPopularKeywordsAsync(cancellationToken);
    }
}