using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobPost.UpdateMyJobPost;

public record UpdateMyJobPostCommand(
    Guid Id,
    UpdateMyJobPostDto Dto
) : IRequest<Result<Unit, Error>>;