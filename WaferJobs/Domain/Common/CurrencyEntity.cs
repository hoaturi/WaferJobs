using WaferJobs.Common;

namespace WaferJobs.Domain.Common;

public class CurrencyEntity : BaseEntity
{
    public int Id { get; set; }
    public required string Label { get; set; }
    public required string Code { get; set; }
    public required string Symbol { get; set; }
    public required decimal Rate { get; set; }
}