using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobPost.GetMyJobPost;

public record GetMyJobPostQuery(Guid Id) : IRequest<Result<GetMyJobPostResponse, Error>>
{
}