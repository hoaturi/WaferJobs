﻿using System.ComponentModel.DataAnnotations;

namespace WaferJobs.Infrastructure.Options;

public class JwtOptions
{
    public const string Key = "Jwt";

    [Required] public required string Issuer { get; init; }
    [Required] public required string Audience { get; init; }
    [Required] public required string AccessKey { get; init; }
    [Required] public required string RefreshKey { get; init; }
    [Required] public required string AccessExpireMinutes { get; init; }
    [Required] public required string RefreshExpireDays { get; init; }
}