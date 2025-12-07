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


        public CourceDbContext(DbContextOptions<CourceDbContext> options)
        : base(options){ }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserRoleEntities>()
                .HasKey(ur => new { ur.userid, ur.roleid });


            modelBuilder.Entity<SubscriptionEntites>()
                .HasKey(s => new { s.followingid, s.followerid });

            modelBuilder.Entity<FavoritEntities>()
               .HasKey(s => new { s.courseid, s.userid });

            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var pk = entityType.FindPrimaryKey();
                if (pk != null)
                {
   
                    var idProperty = pk.Properties.FirstOrDefault(p =>
                        p.ClrType == typeof(int) || p.ClrType == typeof(long));

                    if (idProperty != null)
                    {
                        modelBuilder.Entity(entityType.Name)
                            .Property(idProperty.Name)
                            .ValueGeneratedOnAdd()
                            .UseHiLo();  
                    }
                }
            }

        }


    }
}
