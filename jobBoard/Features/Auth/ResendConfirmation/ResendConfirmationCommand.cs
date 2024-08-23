using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Auth.ResendConfirmation;

public record ResendConfirmationCommand(string Email) : IRequest<Result<Unit, Error>>;