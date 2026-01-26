using Core.Common.EnumS;
using Core.Models.Entitiеs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NpgsqlTypes;
using System.Text.Json;

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
        public DbSet<SubscriptionEntity> subscription { get; set; }
        public DbSet<CategoryEntity> categories { get; set; }
        public DbSet<Course_CategoriesEntity> course_categories { get; set; }
        public DbSet<ReviewreactionEntity> reviewreaction { get; set; }
        public DbSet<ProfileEntity> profiles { get; set; }


        private readonly bool _usePostgreSqlSpecifics;


        public CourceDbContext(DbContextOptions<CourceDbContext> options)
    : this(options, usePostgreSqlSpecifics: true) { }

        public CourceDbContext(DbContextOptions<CourceDbContext> options, bool usePostgreSqlSpecifics)
            : base(options)
        {
            _usePostgreSqlSpecifics = usePostgreSqlSpecifics;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (_usePostgreSqlSpecifics)
            {
                modelBuilder.Entity<CourseEntity>(entity =>
                {
                    entity.Property<NpgsqlTsVector>("searchvector")
                          .HasColumnName("searchvector");
                });

                modelBuilder.HasPostgresEnum<reaction_type>();
            }
            else
            {
                modelBuilder.Entity<CourseEntity>().Ignore("searchvector");
            }

   
            modelBuilder.Entity<ProfileEntity>(entity =>
            {
                entity.ToTable("profiles");
                entity.HasKey(e => e.userid);

         
                var converter = new ValueConverter<Dictionary<string, string>, string>(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = false }),
                    v => string.IsNullOrEmpty(v)
                        ? new Dictionary<string, string>()
                        : JsonSerializer.Deserialize<Dictionary<string, string>>(v, new JsonSerializerOptions { WriteIndented = false }) ?? new Dictionary<string, string>());

                if (_usePostgreSqlSpecifics)
                {
                    entity.Property(e => e.sociallinks)
                        .HasColumnType("jsonb")
                        .HasConversion(converter);
                }
                else
                {
                    entity.Property(e => e.sociallinks)
                        .HasColumnType("text")
                        .HasConversion(converter);
                }

                entity.HasOne(p => p.user)
                    .WithOne()
                    .HasForeignKey<ProfileEntity>(p => p.userid);
            });

            modelBuilder.Entity<ReviewreactionEntity>()
            .Property(e => e.reactiontype)
            .HasConversion<string>();

            modelBuilder.Entity<UserRoleEntity>()
                .HasKey(ur => new { ur.userid, ur.roleid });

            modelBuilder.Entity<SubscriptionEntity>()
                .HasKey(s => new { s.followingid, s.followerid });

            modelBuilder.Entity<FavoritEntity>()
               .HasKey(s => new { s.courseid, s.userid });

            modelBuilder.Entity<Course_CategoriesEntity>()
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
