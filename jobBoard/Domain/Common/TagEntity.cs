namespace JobBoard.Domain.Common;

public class TagEntity
{
    public int Id { get; set; }
    public required string Label { get; set; }
    public required string Slug { get; set; }
}