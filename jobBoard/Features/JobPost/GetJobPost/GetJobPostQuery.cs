using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobPost.GetJobPost;

public record GetJobPostQuery(string Slug) : IRequest<Result<GetJobPostResponse, Error>>;