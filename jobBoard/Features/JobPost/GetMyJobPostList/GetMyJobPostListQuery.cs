using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobPost.GetMyJobPostList;

public record GetMyJobPostListQuery(string? Status, int Page) : IRequest<Result<GetMyJobPostListResponse, Error>>;