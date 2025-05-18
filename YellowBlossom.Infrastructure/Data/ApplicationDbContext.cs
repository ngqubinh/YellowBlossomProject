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
        public DbSet<PMIS_Product> Products { get; set; }
        public DbSet<PMIS_ProjectType> ProjectTypes { get; set; }
        public DbSet<PMIS_ProjectStatus> ProjectStatuses { get; set; }
        public DbSet<PMIS_Project> Projects { get; set; }
        public DbSet<PMIS_PhaseStatus> PhaseStatuses { get; set; }
        public DbSet<PMIS_Phase> Phases { get; set; }
        public DbSet<PMIS_Invite> Invites { get; set; }
        public DbSet<PMIS_InviteW> InvitesW { get; set; }
        public DbSet<PMIS_Priority> Priorities { get; set; }
        public DbSet<PMIS_TaskStatus> TaskStatuses { get; set; }
        public DbSet<PMIS_Task> Tasks { get; set; }
        public DbSet<PMIS_Team> Teams { get; set; }
        public DbSet<PMIS_ProjectTeam> ProjectTeams { get; set; }
        public DbSet<PMIS_UserTeam> UserTeams { get; set; }
        public DbSet<PMIS_TestType> TestTypes { get; set; }
        public DbSet<PMIS_TestCaseStatus> TestCaseStatuses { get; set; }
        public DbSet<PMIS_TestCase> TestCases { get; set; }
        public DbSet<PMIS_TestRunStatus> TestRunStatuses { get; set; }
        public DbSet<PMIS_TestRun> TestRuns { get; set; }
        public DbSet<PMIS_TestRunTestCase> TestRunTestCases { get; set; }
        public DbSet<PMIS_Bug> Bugs { get; set; }

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
            builder.Entity<PMIS_ProjectStatus>()
                .HasMany(u => u.Projects)
                .WithOne(p => p.ProjectStatus)
                .HasForeignKey(p => p.ProjectStatusId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // ProjectType -> Project
            builder.Entity<PMIS_ProjectType>()
                .HasMany(p => p.Projects)
                .WithOne(p => p.ProjectType)
                .HasForeignKey(p => p.ProjectTypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Product -> Project
            builder.Entity<PMIS_Product>()
                .HasMany(p => p.Projects)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // PhaseStatus -> Phase
            builder.Entity<PMIS_PhaseStatus>()
                .HasMany(ps => ps.Phases)
                .WithOne(p => p.PhaseStatus)
                .HasForeignKey(p => p.PhaseStatusId)
                .IsRequired();

            // Project -> Invite
            builder.Entity<PMIS_Project>()
                .HasMany(p => p.InviteTokens)
                .WithOne(i => i.Project)
                .HasForeignKey(i => i.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Project -> Task
            builder.Entity<PMIS_Project>()
                .HasMany(p => p.Tasks)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // TaskStatus -> Task
            builder.Entity<PMIS_TaskStatus>()
                .HasMany(ts => ts.Tasks)
                .WithOne(t => t.TaskStatus)
                .HasForeignKey(t => t.TaskStatusId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Priority -> Task
            builder.Entity<PMIS_Priority>()
                .HasMany(p => p.Tasks)
                .WithOne(t => t.Priority)
                .HasForeignKey(t => t.PriorityId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // User -> Task
            builder.Entity<User>()
                .HasMany(u => u.Tasks)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Team -> Task
            builder.Entity<PMIS_Team>()
                .HasMany(te => te.Tasks)
                .WithOne(ta => ta.Team)
                .HasForeignKey(ta => ta.AssignedTeam)
                .OnDelete(DeleteBehavior.Cascade);

            // Team <-> Project
            builder.Entity<PMIS_ProjectTeam>()
                .HasKey(pt => new { pt.ProjectId, pt.TeamId });

            // User <-> Team
            builder.Entity<PMIS_UserTeam>()
                .HasKey(ut => new {ut.TeamId, ut.UserId });

            // TestType -> TestCase
            builder.Entity<PMIS_TestType>()
                .HasMany(tt => tt.TestCases)
                .WithOne(tc => tc.TestType)
                .HasForeignKey(tc => tc.TestTypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // TestCaseStatus -> TestCase
            builder.Entity<PMIS_TestCaseStatus>()
                .HasMany(tcs => tcs.TestCases)
                .WithOne(tc => tc.TestCaseStatus)
                .HasForeignKey(tc => tc.TestCaseStatusId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Task -> TestCase
            builder.Entity<PMIS_Task>()
                .HasMany(t => t.TestCases)
                .WithOne(tc => tc.Task)
                .HasForeignKey(tc => tc.TaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Team -> TestCase
            builder.Entity<PMIS_Team>()
                .HasMany(t => t.TestCases)
                .WithOne(tc => tc.Team)
                .HasForeignKey(tc => tc.CreateBy)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // TestRunStatus -> TestRun
            builder.Entity<PMIS_TestRunStatus>()
                .HasMany(trs => trs.TestRuns)
                .WithOne(tr => tr.TestRunStatus)
                .HasForeignKey(tr => tr.TestRunStatusId)
                .IsRequired(true);

            // Task -> TestRun
            builder.Entity<PMIS_Task>()
                .HasMany(t => t.TestRuns)
                .WithOne(tr => tr.Task)
                .HasForeignKey(tr => tr.TaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);

            // Team tạo test case -> TestRun
            builder.Entity<PMIS_Team>()
                .HasMany(t => t.CreatedTestRuns)
                .WithOne(tr => tr.CreatedByTeam)
                .HasForeignKey(tr => tr.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Team chạy test case -> TestRun
            builder.Entity<PMIS_Team>()
                .HasMany(t => t.ExecutedTestRuns)
                .WithOne(tr => tr.ExecutedByTeam)
                .HasForeignKey(tr => tr.ExecutedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // TestRun <-> TestCase
            builder.Entity<PMIS_TestRunTestCase>()
                .HasKey(pt => new { pt.TestCaseId, pt.TestRunId });

            // Priority -> Bug
            builder.Entity<PMIS_Priority>()
                .HasMany(p => p.Bugs)
                .WithOne(t => t.Priority)
                .HasForeignKey(t => t.PriorityId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(true);

            // TestCase -> Bug
            builder.Entity<PMIS_TestCase>()
                .HasMany(tc => tc.Bugs)
                .WithOne(b => b.TestCase)
                .HasForeignKey(b => b.TestCaseId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);

            // TestRun -> Bug
            builder.Entity<PMIS_TestRun>()
                .HasMany(tr => tr.Bugs)
                .WithOne(b => b.TestRun)
                .HasForeignKey(b => b.TestRunId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);

            // Team -> Reported Bug
            builder.Entity<PMIS_Team>()
                .HasMany(t => t.BugsReportedBy)
                .WithOne(b => b.ReportedByTeam)
                .HasForeignKey(b => b.ReportedByTeamId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(true);

            // Team -> Assgined Bug
            builder.Entity<PMIS_Team>()
                .HasMany(t => t.BugsAssignedTo)
                .WithOne(b => b.AssignedToTeam)
                .HasForeignKey(b => b.AssignedToTeamId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);


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
