using Application.Abstractions.Repository.Custom;
using Ardalis.Specification;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Common.input;
using infrastructure.DataBase.Context;
using infrastructure.DataBase.Extensions;
using infrastructure.DataBase.Repository.Base;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace infrastructure.DataBase.Repository.Custom
{
    public class CourseRepository : BaseRepository<CourseEntity>, ICourseRepository
    {
        public CourseRepository(CourceDbContext courceDbContext) : base(courceDbContext) { }

        public Task<int> CountOfSearchCourse(
            string searchText, 
            ISpecification<CourseEntity> spec, 
            CancellationToken ct = default)
        {
            var query = GetAllWithoutTracking();

            query = SpecificationEvaluator.GetQuery(query, spec);

            return
                query
                .Where(c => EF.Property<NpgsqlTsVector>(c, "searchvector").Matches(searchText))
                .CountAsync(ct);
        }

        public Task<List<CourseEntity>> GetAllCourse(
            SortingAndPaginationDTO dto, 
            Specification<CourseEntity> spec, 
            string[]? noToSort, 
            CancellationToken ct = default)
        {
            var query = GetAllWithoutTracking();

            query = SpecificationEvaluator.GetQuery(query, spec);

            return 
                query
                .GetWithPaginationAndSorting(dto, noToSort)
                .Include(c => c.user)
                .ToListAsync(ct);
        }


        public async Task<List<CourseEntity>> GetUserCourses(
            int userId, 
            SortingAndPaginationDTO? dto, 
            Specification<CourseEntity> spec, 
            string[]? noToSort, 
            CancellationToken ct = default)
        {
            return 
                await GetAllWithoutTracking()
                        .Include(c => c.user)
                        .GetWithPaginationAndSorting(dto, noToSort)
                        .Where(c => c.creatorid == userId)
                        .ToListAsync(ct);
        }

        public Task<List<CourseEntity>> SearchCourse(
            string searchText, 
            ISpecification<CourseEntity> spec, 
            SortingAndPaginationDTO dto, 
            CancellationToken ct = default)
        {
            var query = GetAllWithoutTracking();

            query = SpecificationEvaluator.GetQuery(query, spec);

            return
            query
           .Where(c => EF.Property<NpgsqlTsVector>(c, "searchvector").Matches(searchText))
           .GetWithPaginationAndSorting(dto)
                .Include(c => c.user)
                .ToListAsync(ct);


 
        }
    }
}
