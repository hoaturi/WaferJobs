using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobAlert.UpdateJobAlert;

public record UpdateJobAlertCommand(
    string Token,
    UpdateJobAlertDto Dto
) : IRequest<Result<Unit, Error>>;