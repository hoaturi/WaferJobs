using Microsoft.AspNetCore.Identity;

namespace JobBoard;

public sealed record EmailDto(ApplicationUser User, string Token);
