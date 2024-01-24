using Microsoft.AspNetCore.Identity;

namespace JobBoard;

public class ApplicationUser : IdentityUser<Guid>
{
    public Business? Business { get; set; }
}
