using MediatR;

namespace JobBoard;

public record GetJobPostQuery(Guid Id) : IRequest<Result<GetJobPostResponse, Error>>;
