namespace JobBoard;

public static class GetJobPostListQueryMapper
{
    public static JobPostDto MapToResponse(JobPost jobPost)
    {
        return new(
            Id: jobPost.Id,
            Category: jobPost.Category.Name,
            Country: jobPost.Country.Name,
            EmploymentType: jobPost.EmploymentType.Name,
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
