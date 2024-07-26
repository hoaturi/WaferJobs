namespace JobBoard.Features.JobAlert.UpdateJobAlert;

public record UpdateJobAlertDto(
    string? Keyword,
    int? CountryId,
    int? EmploymentTypeId,
    int? CategoryId);