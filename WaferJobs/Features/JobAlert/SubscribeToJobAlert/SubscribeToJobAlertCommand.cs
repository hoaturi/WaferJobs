using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.JobAlert.SubscribeToJobAlert;

public record SubscribeToJobAlertCommand(
    string Email,
    string? Keyword,
    int? CountryId,
    List<int>? EmploymentTypeIds,
    List<int>? CategoryIds,
    List<int>? ExperienceLevelIds
) : IRequest<Result<Unit, Error>>;