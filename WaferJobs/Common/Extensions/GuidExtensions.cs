namespace WaferJobs.Common.Extensions;

public static class GuidExtensions
{
    public static string ToBase64String(this Guid guid)
    {
        return Convert.ToBase64String(guid.ToByteArray());
    }

    public static string NewBase64Guid()
    {
        return Guid.NewGuid().ToBase64String();
    }
}