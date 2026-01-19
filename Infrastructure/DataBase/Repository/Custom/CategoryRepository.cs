using Application.Abstractions.Repository.Base;
using Application.Abstractions.Repository.Custom;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Common.input;
using infrastructure.DataBase.Context;
using infrastructure.DataBase.Extensions;
using infrastructure.DataBase.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.DataBase.Repository.Custom
{
    public class CategoryRepository : BaseRepository<Course_CategoriesEntity>, ICategoryRepository
    {
        public readonly IBaseRepository<CategoryEntity> _categoryRepo;

        public CategoryRepository(
            CourceDbContext courceDbContext,
            BaseRepository<CategoryEntity> categoryRepo
            ) 
            : base(courceDbContext) 
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<int> CountBySearch(string searchText)
        {

           return await _categoryRepo.GetAllWithoutTracking()
                .Where(c => EF.Functions.ILike(c.name, $"%{searchText}%"))
                .CountAsync();
        }

        public async Task<Dictionary<int, Dictionary<int, string>>> GetCategoryNamesForCourses(
            List<int> courseIds,
            PaginationDTO pagination,
            CancellationToken ct = default)
        {
            

        }

        public async Task<List<CategoryEntity>> SearchCategory(
            string searchText, 
            PaginationDTO pagination, 
            CancellationToken ct = default)
        {
            return await _categoryRepo.GetAllWithoutTracking()
                        .Where(c => EF.Functions.ILike(c.name, $"%{searchText}%"))
                        .GetWithPagination(pagination)
                            .OrderBy(c => c.parentid)
                            .OrderBy(c => c.name)
                            .ToListAsync();

        }
    }
}
