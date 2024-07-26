using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobAlert.UnsubscribeToJobAlert;

public record UnsubscribeToJobAlertCommand(string Token) : IRequest<Result<Unit, Error>>;