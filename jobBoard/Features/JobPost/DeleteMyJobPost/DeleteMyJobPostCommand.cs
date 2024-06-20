using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobPost.DeleteMyJobPost;

public record DeleteMyJobPostCommand(Guid Id) : IRequest<Result<Unit, Error>>;