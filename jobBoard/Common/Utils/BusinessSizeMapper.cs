namespace JobBoard;

public static class BusinessSizeMapper
{
    /// <summary>
    /// Maps the specified business size value to its corresponding string representation.
    /// </summary>
    /// <param name="size">The business size value to be mapped.</param>
    /// <returns>The string representation of the mapped business size value.</returns>
    public static string MapToString(int? size)
    {
        if (size is null)
        {
            return null!;
        }

        if (size.Value < 1 || size.Value > BusinessSize.BusinessSizes.Count)
        {
            throw new InvalidBusinessSizeException(size.Value);
        }

        var businessSize = BusinessSize.BusinessSizes[size.Value];
        return businessSize;
    }
}
