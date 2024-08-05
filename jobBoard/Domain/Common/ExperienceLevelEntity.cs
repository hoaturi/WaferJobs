namespace JobBoard.Domain.Common;

public class ExperienceLevelEntity
{
    public int Id { get; set; }
    public required string Label { get; set; }
    public required string Slug { get; set; }
}