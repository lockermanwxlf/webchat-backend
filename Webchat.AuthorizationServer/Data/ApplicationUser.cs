using Microsoft.AspNetCore.Identity;

namespace Webchat.AuthorizationServer.Data
{
    public class ApplicationUser : IdentityUser<int>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}
