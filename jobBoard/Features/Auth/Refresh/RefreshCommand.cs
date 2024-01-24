using MediatR;

namespace JobBoard;

public record RefreshCommand(string RefreshToken) : IRequest<Result<RefreshResponse, Error>>;
