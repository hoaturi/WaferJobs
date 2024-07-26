using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobAlert.UpdateJobAlert;

public record UpdateJobAlertCommand(
    string Token,
    string? Keyword,
    int? CountryId,
    int? EmploymentTypeId,
    int? CategoryId) : IRequest<Result<Unit, Error>>;