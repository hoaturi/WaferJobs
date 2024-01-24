using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace JobBoard;

public class Business : BaseEntity
{
    public Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required string Name { get; set; }
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Size { get; set; }
    public string? StripeCustomerId { get; set; }
    public string? Url { get; set; }
    public string? TwitterUrl { get; set; }
    public string? LinkedInUrl { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public List<JobPost>? JobPosts { get; set; }
}
