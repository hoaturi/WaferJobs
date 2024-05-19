using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobPost.PublishFeaturedJobPost;

public record PublishFeaturedJobPostCommand(string StripeEventId, string SessionId)
    : IRequest<Result<Unit, Error>>;