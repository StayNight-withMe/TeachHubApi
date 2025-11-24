using infrastructure.Entitiеs;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Context
{
    public class CourceDbContext : DbContext
    {
        public DbSet<UserRoleEntities> usersroles { get; set; }
        public DbSet<UserEntities> users { get; set; }
        public DbSet<FavoritEntities> favorit { get; set; }
        public DbSet<RoleEntities> roles { get; set; }
        public DbSet<ReviewEntities> reviews { get; set; }
        public DbSet<LessonEntities> lesson { get; set; }
        public DbSet<CourseEntities> courses { get; set; } 
        public DbSet<ChapterEntity> chapter { get; set; }
        public DbSet<LessonfilesEntitits> lessonfiles { get; set; }

        public CourceDbContext(DbContextOptions<CourceDbContext> options)
        : base(options){ }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserRoleEntities>()
                .HasKey(ur => new { ur.userid, ur.roleid });


            modelBuilder.Entity<SubscriptionEntites>()
                .HasKey(s => new { s.followingid, s.followerid });

            base.OnModelCreating(modelBuilder);
        }


    }
}
