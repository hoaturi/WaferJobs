﻿using JobBoard.Common;

namespace JobBoard.Domain.Conference;

public class ConferenceEntity : BaseEntity
{
    public int Id { get; set; }
    public required string ContactEmail { get; set; }
    public required string ContactName { get; set; }
    public required string Title { get; set; }
    public required string OrganizerName { get; set; }
    public required string OrganizerEmail { get; set; }
    public required string Location { get; set; }
    public required string WebsiteUrl { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsVerified { get; set; }
    public bool IsPublished { get; set; }
}