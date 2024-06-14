using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Webchat.AuthorizationServer.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, int>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationRole>()
                .Property(p => p.ConcurrencyStamp)
                .IsETagConcurrency();
            builder.Entity<ApplicationUser>()
                .Property(p => p.ConcurrencyStamp)
                .IsETagConcurrency();
        }
    }
}
