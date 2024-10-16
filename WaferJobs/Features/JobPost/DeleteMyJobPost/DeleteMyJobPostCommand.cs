using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.JobPost.DeleteMyJobPost;

public record DeleteMyJobPostCommand(Guid Id) : IRequest<Result<Unit, Error>>;