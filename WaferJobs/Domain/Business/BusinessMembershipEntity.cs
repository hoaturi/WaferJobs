﻿using WaferJobs.Common;
using WaferJobs.Domain.Auth;

namespace WaferJobs.Domain.Business;

public class BusinessMembershipEntity : BaseEntity
{
    public Guid Id { get; set; }
    public Guid BusinessId { get; set; }
    public Guid UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Title { get; set; }
    public string? stripeCustomerId { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; }
    public BusinessEntity Business { get; set; } = null!;
    public ApplicationUserEntity User { get; set; } = null!;
}