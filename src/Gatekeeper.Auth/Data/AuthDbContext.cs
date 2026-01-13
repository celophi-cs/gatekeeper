using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gatekeeper.Auth.Data
{
    // Use IdentityDbContext for Identity and OpenIddict integration
    public class AuthDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        // Add additional DbSets for custom entities if needed

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cosmos DB: Remove ConcurrencyStamp as concurrency token for IdentityUser
            modelBuilder.Entity<IdentityUser>()
                .Property(u => u.ConcurrencyStamp)
                .IsConcurrencyToken(false);
            modelBuilder.Entity<IdentityUser>()
                .Property<string>("_etag")
                .IsETagConcurrency();

            // Cosmos DB: Remove ConcurrencyStamp as concurrency token for IdentityRole
            modelBuilder.Entity<IdentityRole>()
                .Property(r => r.ConcurrencyStamp)
                .IsConcurrencyToken(false);
            modelBuilder.Entity<IdentityRole>()
                .Property<string>("_etag")
                .IsETagConcurrency();

            // Cosmos DB: Map IdentityUser to Users container with /id partition key
            modelBuilder.Entity<IdentityUser>()
                .ToContainer("Users")
                .HasPartitionKey(u => u.Id);

            // Cosmos DB: Map IdentityRole to Users container (or separate if needed)
            modelBuilder.Entity<IdentityRole>()
                .ToContainer("Users")
                .HasPartitionKey(r => r.Id);

            // Register OpenIddict entities
            modelBuilder.UseOpenIddict();

            // Cosmos DB: Remove ConcurrencyToken for OpenIddict entities and use _etag
            modelBuilder.Entity("OpenIddictEntityFrameworkCoreApplication")
                .Property<string>("ConcurrencyToken").IsConcurrencyToken(false);
            modelBuilder.Entity("OpenIddictEntityFrameworkCoreAuthorization")
                .Property<string>("ConcurrencyToken").IsConcurrencyToken(false);
            modelBuilder.Entity("OpenIddictEntityFrameworkCoreScope")
                .Property<string>("ConcurrencyToken").IsConcurrencyToken(false);
            modelBuilder.Entity("OpenIddictEntityFrameworkCoreToken")
                .Property<string>("ConcurrencyToken").IsConcurrencyToken(false);
            modelBuilder.Entity("OpenIddictEntityFrameworkCoreApplication")
                .Property<string>("_etag").IsETagConcurrency();
            modelBuilder.Entity("OpenIddictEntityFrameworkCoreAuthorization")
                .Property<string>("_etag").IsETagConcurrency();
            modelBuilder.Entity("OpenIddictEntityFrameworkCoreScope")
                .Property<string>("_etag").IsETagConcurrency();
            modelBuilder.Entity("OpenIddictEntityFrameworkCoreToken")
                .Property<string>("_etag").IsETagConcurrency();
        }
    }
}
