﻿namespace WaferJobs.Infrastructure.Services.CurrentUserService;

public interface ICurrentUserService
{
    public Guid GetUserId();

    public Guid? TryGetUserId();

    public string GetUserEmail();
}