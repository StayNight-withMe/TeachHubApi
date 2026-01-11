using Application.Abstractions.Repository.Base;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Common.input;

namespace Application.Abstractions.Repository.Custom
{
    public interface ICategoryRepository : IBaseRepository<Course_CategoriesEntities>
    {
        public Task<Dictionary<int, Dictionary<int, string>>> GetCategoryNamesForCourses(
            List<int> courseIds,
            CancellationToken ct = default);
            
        
        public Task<List<CategoryEntity>> SearchCategory(
            string searchText, 
            PaginationDTO pagination,
            CancellationToken ct = default);

        public Task<int> CountBySearch(string searchText);

            //            .GetAllWithoutTracking()
            //    .Where(c => EF.Functions.ILike(c.name, $"%{searchText}%"));

            //var resultEntities = await qwery.GetWithPagination(pagination)
            //    .OrderBy(c => c.parentid)
            //    .OrderBy(c => c.name)
            //    .ToListAsync();

    }
}
