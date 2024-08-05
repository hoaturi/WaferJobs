namespace JobBoard.Domain.JobPost;

public class CategoryEntity
{
    public int Id { get; set; }
    public required string Label { get; init; }
    public required string Slug { get; init; }
}