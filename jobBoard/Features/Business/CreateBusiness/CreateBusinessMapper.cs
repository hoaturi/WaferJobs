namespace JobBoard;

public static class CreateBusinessMapper
{
    public static Business MapToEntity(CreateBusinessCommand command, Guid userId)
    {
        return new Business
        {
            Name = command.Name,
            Description = command.Description,
            Location = command.Location,
            Size = BusinessSizeMapper.MapToString(command.Size),
            Url = command.Url,
            TwitterUrl = command.TwitterUrl,
            LinkedInUrl = command.LinkedInUrl,
            UserId = userId
        };
    }
}
