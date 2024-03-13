namespace JobBoard;

public static class CreateFeaturedJobPostCommandMapper
{
    public static JobPost MapToEntity(CreateFeaturedJobPostCommand request, Business business)
    {
        var jobPost = new JobPost
        {
            CategoryId = request.CategoryId,
            CountryId = request.CountryId,
            EmploymentTypeId = request.EmploymentTypeId,
            Title = request.Title,
            Description = request.Description,
            CompanyName = business!.Name,
            CompanyLogoUrl = business.LogoUrl,
            ApplyUrl = request.ApplyUrl,
            IsRemote = request.IsRemote,
            City = request.City,
            MinSalary = request.MinSalary,
            MaxSalary = request.MaxSalary,
            Currency = request.Currency,
            BusinessId = business.Id,
            Tags = request.Tags,
            IsFeatured = true,
            IsPublished = false
        };

        return jobPost;
    }
}
