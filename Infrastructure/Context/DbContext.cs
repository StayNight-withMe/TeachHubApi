using Core.Common;
using infrastructure.Entitiеs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

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
        public DbSet<LessonfilesEntities> lessonfiles { get; set; }
        public DbSet<SubscriptionEntites> subscription { get; set; }
        public DbSet<CategoriesEntities> categories { get; set; }
        public DbSet<Course_CategoriesEntities> course_categories { get; set; }
        public DbSet<ReviewreactionEntities> reviewreaction { get; set; }

        public CourceDbContext(DbContextOptions<CourceDbContext> options)
        : base(options){ }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            modelBuilder.HasPostgresEnum<reaction_type>();



            modelBuilder.Entity<ReviewreactionEntities>()
            .Property(e => e.reactiontype)
            .HasConversion<string>();

            modelBuilder.Entity<UserRoleEntities>()
                .HasKey(ur => new { ur.userid, ur.roleid });

            modelBuilder.Entity<SubscriptionEntites>()
                .HasKey(s => new { s.followingid, s.followerid });

            modelBuilder.Entity<FavoritEntities>()
               .HasKey(s => new { s.courseid, s.userid });

            modelBuilder.Entity<Course_CategoriesEntities>()
               .HasKey(s => new { s.courseid, s.categoryid });


            base.OnModelCreating(modelBuilder);

            //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            //{
            //    var pk = entityType.FindPrimaryKey();
            //    if (pk != null)
            //    {
   
            //        var idProperty = pk.Properties.FirstOrDefault(p =>
            //            p.ClrType == typeof(int) || p.ClrType == typeof(long));

            //        if (idProperty != null)
            //        {
            //            modelBuilder.Entity(entityType.Name)
            //                .Property(idProperty.Name)
            //                .ValueGeneratedOnAdd()
            //                .UseHiLo();  
            //        }
                //}
            //}

        }


    }
}
