namespace WaferJobs.Features.JobAlert.GetJobAlert;

public record GetJobAlertResponse(
    string Email,
    string? Keyword,
    int? CountryId,
    List<int>? ExperienceLevelId,
    List<int>? CategoriesIds,
    List<int>? EmploymentTypesIds
);