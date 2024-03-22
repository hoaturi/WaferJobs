namespace JobBoard;

public static class GetJobPostQueryMapper
{
    public static GetJobPostResponse MapToResponse(JobPost jobPost)
    {
        return new(
            Id: jobPost.Id,
            Category: jobPost.Category.Label,
            Country: jobPost.Country.Label,
            EmploymentType: jobPost.EmploymentType.Label,
            Title: jobPost.Title,
            Description: jobPost.Description,
            IsRemote: jobPost.IsRemote,
            IsFeatured: jobPost.IsFeatured,
            CompanyName: jobPost.CompanyName,
            City: jobPost.City,
            MinSalary: jobPost.MinSalary,
            MaxSalary: jobPost.MaxSalary,
            Currency: jobPost.Currency,
            ApplyUrl: jobPost.ApplyUrl,
            BusinessId: jobPost.BusinessId,
            CompanyLogoUrl: jobPost.CompanyLogoUrl,
            PublishedAt: jobPost.PublishedAt
        );
    }
}
