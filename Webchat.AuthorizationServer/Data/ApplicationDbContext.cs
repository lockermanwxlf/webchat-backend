using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace Webchat.AuthorizationServer.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
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
            builder.Entity<OpenIddictEntityFrameworkCoreApplication>()
                .Property(p => p.ConcurrencyToken)
                .IsETagConcurrency();
            builder.Entity<OpenIddictEntityFrameworkCoreAuthorization>()
                .Property(p => p.ConcurrencyToken)
                .IsETagConcurrency();
            builder.Entity<OpenIddictEntityFrameworkCoreScope>()
                .Property(p => p.ConcurrencyToken)
                .IsETagConcurrency();
            builder.Entity<OpenIddictEntityFrameworkCoreToken>()
                .Property(p => p.ConcurrencyToken)
                .IsETagConcurrency();            
        }
    }
}
