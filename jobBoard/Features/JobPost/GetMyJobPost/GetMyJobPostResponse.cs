﻿namespace JobBoard.Features.JobPost.GetMyJobPost;

public record GetMyJobPostResponse(
    Guid Id,
    string Category,
    string Country,
    string EmploymentType,
    string Title,
    string Description,
    bool IsRemote,
    bool IsFeatured,
    string CompanyName,
    string? City,
    int? MinSalary,
    int? MaxSalary,
    string? Currency,
    string? ApplyUrl,
    Guid? BusinessId,
    string? CompanyLogoUrl,
    string? CompanyWebsiteUrl,
    List<string>? Tags
);