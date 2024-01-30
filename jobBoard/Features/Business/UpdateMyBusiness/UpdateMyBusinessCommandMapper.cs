namespace JobBoard;

public static class UpdateMyBusinessCommandMapper
{
    public static Business MapToEntity(UpdateMyBusinessCommand request, Business business)
    {
        business.Name = request.Name;
        business.Description = request.Description;
        business.Location = request.Location;
        business.BusinessSizeId = request.BusinessSizeId;
        business.Url = request.Url;
        business.TwitterUrl = request.TwitterUrl;
        business.LinkedInUrl = request.LinkedInUrl;

        return business;
    }
}
