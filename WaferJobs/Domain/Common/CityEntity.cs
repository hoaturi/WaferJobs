using WaferJobs.Common;

namespace WaferJobs.Domain.Common;

public class CityEntity : BaseEntity
{
    public int Id { get; set; }
    public required string Label { get; init; }
    public required string Slug { get; init; }
}