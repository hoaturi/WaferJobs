using JobBoard.Common;
using JobBoard.Domain.JobPost;

namespace JobBoard.Domain.Business;

public class BusinessEntity : BaseEntity
{
    public Guid Id { get; init; }
    public int? BusinessSizeId { get; set; }
    public required string Name { get; set; }
    public required string WebsiteUrl { get; set; }
    public required string Domain { get; set; }
    public required string Slug { get; set; }
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? TwitterUrl { get; set; }
    public string? LinkedinUrl { get; set; }
    public bool IsClaimed { get; set; }
    public bool IsActive { get; set; }
    public List<BusinessMembershipEntity> Memberships { get; init; } = [];
    public List<JobPostEntity> JobPosts { get; init; } = [];
    public BusinessSizeEntity? BusinessSize { get; init; }
}