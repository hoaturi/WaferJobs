namespace JobBoard.Domain.Common;

public class CountryEntity
{
    public int Id { get; set; }
    public required string Label { get; init; }
    public required string Code { get; init; }
    public required string Slug { get; init; }
}