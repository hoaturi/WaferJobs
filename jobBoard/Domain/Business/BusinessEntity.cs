﻿using JobBoard.Common;
using JobBoard.Domain.JobPost;

namespace JobBoard.Domain.Business;

public class BusinessEntity : BaseEntity
{
    public Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public int? BusinessSizeId { get; set; }
    public required string Name { get; set; }
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? StripeCustomerId { get; set; }
    public string? Url { get; set; }
    public string? TwitterUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public BusinessUserEntity? User { get; init; }
    public List<JobPostEntity>? JobPosts { get; init; }
    public BusinessSizeEntity? BusinessSize { get; init; }
}