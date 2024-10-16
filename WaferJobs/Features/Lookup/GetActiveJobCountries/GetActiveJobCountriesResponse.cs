namespace WaferJobs.Features.Lookup.GetActiveJobCountries;

public record ActiveJobCountryDto(int Id, string Label, string Slug);

public record GetActiveJobCountriesResponse(IReadOnlyList<ActiveJobCountryDto> Countries);