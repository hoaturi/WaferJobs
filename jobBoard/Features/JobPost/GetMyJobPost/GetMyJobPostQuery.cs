using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobPost.GetMyJobPost;

public record GetMyJobPostQuery(string Slug) : IRequest<Result<GetMyJobPostResponse, Error>>;