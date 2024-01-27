namespace JobBoard;

public static class UpdateBusinessCommandMapper
{
    public static Business MapToEntity(UpdateBusinessCommand request, Business business)
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
