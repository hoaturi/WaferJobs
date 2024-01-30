using MediatR;

namespace JobBoard;

public record GetMyBusinessQuery : IRequest<Result<GetBusinessResponse, Error>>;
