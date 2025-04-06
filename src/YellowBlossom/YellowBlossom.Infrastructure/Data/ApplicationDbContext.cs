using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YellowBlossom.Domain.Models.Auth;
using YellowBlossom.Domain.Models.PMIS;

namespace YellowBlossom.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Set the tables
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProjectType> ProjectTypes { get; set; }
        public DbSet<ProjectStatus> ProjectStatuses { get; set; }
        public DbSet<Project> Projects { get; set; }

        // Configure
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Configure relationships
            // User -> RefreshToken
            builder.Entity<User>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // User -> Product
            builder.Entity<User>()
                .HasMany(u => u.Products)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // User -> Project
            builder.Entity<User>()
                .HasMany(u => u.Projects)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // ProjectStatus -> Project
            builder.Entity<ProjectStatus>()
                .HasMany(u => u.Projects)
                .WithOne(p => p.ProjectStatus)
                .HasForeignKey(p => p.ProjectStatusId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // ProjectType -> Project
            builder.Entity<ProjectType>()
                .HasMany(p => p.Projects)
                .WithOne(p => p.ProjectType)
                .HasForeignKey(p => p.ProjectTypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Product -> Project
            builder.Entity<Product>()
                .HasMany(p => p.Projects)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Configure Identity library
            builder.Entity<User>(e => { e.ToTable("User"); });
            builder.Entity<IdentityUserClaim<string>>(e => { e.ToTable("UserClaims"); });
            builder.Entity<IdentityUserLogin<string>>(e => { e.ToTable("UserLogins"); });
            builder.Entity<IdentityUserToken<string>>(e => { e.ToTable("UserTokens"); });
            builder.Entity<IdentityRole>(e => { e.ToTable("Roles"); });
            builder.Entity<IdentityRoleClaim<string>>(e => { e.ToTable("RoleClaims"); });
            builder.Entity<IdentityUserRole<string>>(e => { e.ToTable("UserRoles"); });
        }

    }
}
