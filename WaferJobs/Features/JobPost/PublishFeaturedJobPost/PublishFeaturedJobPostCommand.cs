using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.JobPost.PublishFeaturedJobPost;

public record PublishFeaturedJobPostCommand(string StripeEventId, string SessionId)
    : IRequest<Result<Unit, Error>>;