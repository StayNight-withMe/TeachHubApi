using Core.Common.EnumS;
using Core.Models.Entitiеs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace infrastructure.DataBase.Context
{
    public class CourceDbContext : DbContext
    {
        public DbSet<UserRoleEntity> usersroles { get; set; }
        public DbSet<UserEntity> users { get; set; }
        public DbSet<FavoritEntity> favorit { get; set; }
        public DbSet<RoleEntity> roles { get; set; }
        public DbSet<ReviewEntity> reviews { get; set; }
        public DbSet<LessonEntity> lesson { get; set; }
        public DbSet<CourseEntity> courses { get; set; } 
        public DbSet<ChapterEntity> chapter { get; set; }
        public DbSet<LessonfileEntity> lessonfiles { get; set; }
        public DbSet<SubscriptionEntites> subscription { get; set; }
        public DbSet<CategoryEntity> categories { get; set; }
        public DbSet<Course_CategoriesEntities> course_categories { get; set; }
        public DbSet<ReviewreactionEntity> reviewreaction { get; set; }
        public DbSet<ProfileEntity> profiles { get; set; }

        public CourceDbContext(DbContextOptions<CourceDbContext> options)
        : base(options){ }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            modelBuilder.HasPostgresEnum<reaction_type>();


            modelBuilder.Entity<ProfileEntity>(entity =>
            {
                entity.ToTable("profiles");
                entity.HasKey(e => e.userid);

                entity.Property(e => e.sociallinks).HasColumnType("jsonb");

                entity.HasOne(p => p.user)
                    .WithOne() 
                    .HasForeignKey<ProfileEntity>(p => p.userid);
            });

            modelBuilder.Entity<ReviewreactionEntity>()
            .Property(e => e.reactiontype)
            .HasConversion<string>();

            modelBuilder.Entity<UserRoleEntity>()
                .HasKey(ur => new { ur.userid, ur.roleid });

            modelBuilder.Entity<SubscriptionEntites>()
                .HasKey(s => new { s.followingid, s.followerid });

            modelBuilder.Entity<FavoritEntity>()
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
