using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.JobPost.UpdateMyJobPost;

public record UpdateMyJobPostCommand(
    Guid Id,
    UpdateMyJobPostDto Dto
) : IRequest<Result<Unit, Error>>;