using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobAlert.SubscribeToJobAlert;

public record SubscribeToJobAlertCommand(
    string Email,
    string? Keyword,
    int? CountryId,
    int? EmploymentTypeId,
    int? CategoryId
) : IRequest<Result<Unit, Error>>;