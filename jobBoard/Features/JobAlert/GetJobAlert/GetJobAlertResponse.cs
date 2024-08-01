namespace JobBoard.Features.JobAlert.GetJobAlert;

public record GetJobAlertResponse(
    string Email,
    string? Keyword,
    int? CountryId,
    List<int>? CategoriesIds,
    List<int>? EmploymentTypesIds
);