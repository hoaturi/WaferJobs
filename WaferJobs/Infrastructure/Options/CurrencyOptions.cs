﻿using System.ComponentModel.DataAnnotations;

namespace WaferJobs.Infrastructure.Options;

public class CurrencyOptions
{
    public const string Key = "CurrencyApi";

    [Required] public required string ApiKey { get; set; }
    [Required] public required string BaseUrl { get; set; }
}