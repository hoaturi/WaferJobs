using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobAlert.GetJobAlert;

public record GetJobAlertQuery(string Token) : IRequest<Result<GetJobAlertResponse, Error>>;