using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Auth.Refresh;

public record RefreshCommand(string RefreshToken) : IRequest<Result<RefreshResponse, Error>>;