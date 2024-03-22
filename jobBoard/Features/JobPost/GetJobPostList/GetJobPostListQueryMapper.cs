namespace JobBoard;

public static class GetJobPostListQueryMapper
{
    public static JobPostDto MapToResponse(JobPost jobPost)
    {
        return new(
            Id: jobPost.Id,
            Category: jobPost.Category.Label,
            Country: jobPost.Country.Label,
            EmploymentType: jobPost.EmploymentType.Label,
            Title: jobPost.Title,
            IsRemote: jobPost.IsRemote,
            IsFeatured: jobPost.IsFeatured,
            CompanyName: jobPost.CompanyName,
            City: jobPost.City,
            MinSalary: jobPost.MinSalary,
            MaxSalary: jobPost.MaxSalary,
            Currency: jobPost.Currency,
            BusinessId: jobPost.BusinessId,
            CompanyLogoUrl: jobPost.CompanyLogoUrl,
            PublishedAt: jobPost.PublishedAt,
            Tags: jobPost.Tags ?? []
        );
    }
}
