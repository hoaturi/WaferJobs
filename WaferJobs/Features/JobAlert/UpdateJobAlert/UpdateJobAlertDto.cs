﻿namespace WaferJobs.Features.JobAlert.UpdateJobAlert;

public record UpdateJobAlertDto(
    string? Keyword,
    int? CountryId,
    List<int>? EmploymentTypeIds,
    List<int>? CategoryIds,
    List<int>? ExperienceLevelIds);