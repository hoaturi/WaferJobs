namespace JobBoard;

public static class CreateJobPostCommandMapper
{
    public static JobPost MapToEntity(CreateJobPostCommand request, Business business)
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
            IsFeatured = request.IsFeatured,
            City = request.City,
            MinSalary = request.MinSalary,
            MaxSalary = request.MaxSalary,
            Currency = request.Currency,
            BusinessId = business.Id,
            IsPublished = false
        };

        return jobPost;
    }
}
