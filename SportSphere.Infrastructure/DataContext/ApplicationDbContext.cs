using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;

namespace SportSphere.Infrastructure.DataContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<SocialProfile> SocialProfiles { get; set; } = null!;
        public DbSet<AthleteProfile> AthleteProfiles { get; set; } = null!;
        public DbSet<AthleteAchievement> AthleteAchievements { get; set; } = null!;
        public DbSet<Follow> Follows { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<PostComment> PostComments { get; set; } = null!;
        public DbSet<PostLike> PostLikes { get; set; } = null!;
        public DbSet<UserBlock> UserBlocks { get; set; } = null!;
        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            #region setup relations
            // One-to-one: ApplicationUser <-> SocialProfile
            builder.Entity<ApplicationUser>()
                .HasOne(a => a.SocialProfile)
                .WithOne(s => s.User)
                .HasForeignKey<SocialProfile>(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Self-referencing many-to-many: Follow
            builder.Entity<Follow>()
     .HasOne(f => f.Follower)
     .WithMany(s => s.Following)
     .HasForeignKey(f => f.FollowerId)
     .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Follow>()
                .HasOne(f => f.Following)
                .WithMany(s => s.Followers)
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.NoAction);

            // AthleteProfile -> AthleteAchievements (one-to-many)
            builder.Entity<AthleteAchievement>()
     .HasOne(a => a.Athlete)
     .WithMany(ap => ap.AthleteAchievements)
     .HasForeignKey(a => a.AthleteId)
     .OnDelete(DeleteBehavior.NoAction);

            // Post relationships
            builder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany(s => s.Posts)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PostComment>()
                .HasOne(c => c.User)
                .WithMany(s => s.PostComments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PostComment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PostLike>()
                .HasOne(l => l.User)
                .WithMany(s => s.PostLikes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PostLike>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<MediaFolder>()
    .HasMany(f => f.MediaFiles)
    .WithOne(m => m.Folder)
    .HasForeignKey(m => m.FolderId)
    .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<MediaFolder>()
    .HasOne(f => f.User)
    .WithMany(u => u.MediaFolders)
    .HasForeignKey(f => f.UserId)
    .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Post>()
                .HasMany(p => p.MediaFiles)
                .WithOne(m => m.Post)
                .HasForeignKey(m => m.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<UserBlock>()
        .HasOne(ub => ub.Blocker)
        .WithMany(u => u.Blocker) // navigation in ApplicationUser
        .HasForeignKey(ub => ub.BlockerId)
        .OnDelete(DeleteBehavior.NoAction); // prevent cascade delete issues

            builder.Entity<UserBlock>()
                .HasOne(ub => ub.Blocked)
                .WithMany(u => u.Blocked) // navigation in ApplicationUser
                .HasForeignKey(ub => ub.BlockedId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<Report>()
    .HasOne(r => r.Reporter)
    .WithMany(u => u.ReportsMade)
    .HasForeignKey(r => r.ReporterId)
    .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Report>()
                .HasOne(r => r.ReportedUser)
                .WithMany(u => u.ReportsReceived)
                .HasForeignKey(r => r.ReportedUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Report>()
                .HasOne(r => r.ReportedPost)
                .WithMany(p => p.Reports)
                .HasForeignKey(r => r.ReportedPostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Report>()
                .HasOne(r => r.ReportedComment)
                .WithMany(c => c.Reports)
                .HasForeignKey(r => r.ReportedCommentId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region seedRoles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "SuperAdmin", NormalizedName = "SUPERADMIN" },
                new IdentityRole { Id = "2", Name = "Athlete", NormalizedName = "ATHLETE" },
                new IdentityRole { Id = "3", Name = "Scout", NormalizedName = "SCOUT" },
                new IdentityRole { Id = "4", Name = "Coach", NormalizedName = "COACH" },
                new IdentityRole { Id = "5", Name = "Fan", NormalizedName = "FAN" }
            );
            #endregion


            #region seedAdmin
            var adminId = Guid.NewGuid().ToString();
            var hasher = new PasswordHasher<ApplicationUser>();
            var adminUser = new ApplicationUser
            {
                Id = adminId,
                UserName = "hobv@gmail.com",
                NormalizedUserName = "HOBV@GMAIL.COM",
                Email = "hobv@gmail.com",
                NormalizedEmail = "HOBV@GMAIL.COM",
                FullName = "hobv",
                Gender = GenderType.Male,
                DateOfBirth = DateTime.Parse("1998-09-19T00:00:00.000Z"),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Aa123456");

            builder.Entity<ApplicationUser>().HasData(adminUser);
            #endregion

            #region AssignAdminRole

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = "1",
                UserId = adminId
            });
            #endregion



        }
    }
}
