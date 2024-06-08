using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Auth.CheckEmailAvailability;

public record CheckEmailAvailabilityQuery(string Email) : IRequest<Result<Unit, Error>>;