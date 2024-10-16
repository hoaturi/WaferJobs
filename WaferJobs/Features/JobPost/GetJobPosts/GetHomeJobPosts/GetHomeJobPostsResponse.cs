namespace WaferJobs.Features.JobPost.GetJobPosts.GetHomeJobPosts;

public record GetHomeJobPostsResponse(List<JobPostDto> FeaturedJobs, List<JobPostDto> LatestJobs);