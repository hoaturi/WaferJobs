namespace JobBoard.Domain.JobPostEntities;

public class EmploymentTypeEntity
{
    public int Id { get; set; }
    public required string Label { get; set; }
    public required string Slug { get; set; }
}