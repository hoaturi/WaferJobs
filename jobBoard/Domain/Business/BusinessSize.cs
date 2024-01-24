namespace JobBoard;

public class BusinessSize
{
    public static readonly Dictionary<int, string> BusinessSizes =
        new()
        {
            { 1, "1-10" },
            { 2, "11-50" },
            { 3, "51-200" },
            { 4, "201-500" },
            { 5, "501-1000" },
            { 6, "1001-5000" },
            { 7, "5001-10000" },
            { 8, "10001+" }
        };
}
