using MediatR;

namespace JobBoard;

public record GetBusinessQuery(Guid Id) : IRequest<Result<GetBusinessResponse, Error>>;
