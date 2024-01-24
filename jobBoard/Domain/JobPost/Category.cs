using System.ComponentModel.DataAnnotations;

namespace JobBoard;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
}
