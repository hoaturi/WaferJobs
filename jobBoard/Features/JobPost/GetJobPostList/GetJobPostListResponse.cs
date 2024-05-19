using JobBoard.Features.JobPost.GetJobPost;

namespace JobBoard.Features.JobPost.GetJobPostList;

public record GetJobPostListResponse(List<GetJobPostResponse> JobPostList, int Total);