namespace JobBoard;

public static class UpdateBusinessMapper
{
    public static Business MapToEntity(UpdateBusinessCommand command, Business business)
    {
        business.Name = command.Name;
        business.Description = command.Description;
        business.Location = command.Location;
        business.Url = command.Url;
        business.TwitterUrl = command.TwitterUrl;
        business.LinkedInUrl = command.LinkedInUrl;
        business.Size = BusinessSizeMapper.MapToString(command.Size);

        return business;
    }
}
