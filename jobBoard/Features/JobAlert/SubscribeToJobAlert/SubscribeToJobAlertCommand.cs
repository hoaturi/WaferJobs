using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobAlert.SubscribeToJobAlert;

public record SubscribeToJobAlertCommand(
    string Email,
    string? Keyword,
    int? CountryId,
    List<int>? EmploymentTypeIds,
    List<int>? CategoryIds,
    List<int>? ExperienceLevelIds
) : IRequest<Result<Unit, Error>>;