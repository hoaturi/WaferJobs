using MediatR;

namespace JobBoard;

public record PublishFeaturedJobPostCommand(string StripeEventId, string SessionId)
    : IRequest<Result<Unit, Error>>;
