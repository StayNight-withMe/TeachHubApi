using Application.Abstractions.Repository.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using infrastructure.DataBase.Repository.Base;
using Core.Models.Entitiеs;
using infrastructure.DataBase.Context;
using Core.Models.TargetDTO.Common.input;

namespace infrastructure.DataBase.Repository.Custom
{
    public class CategoryRepository : BaseRepository<Course_CategoriesEntity>, ICategoryRepository
    {

        public CategoryRepository(CourceDbContext courceDbContext) : base(courceDbContext)
        {
        }

        public Task<int> CountBySearch(string searchText)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<int, Dictionary<int, string>>> GetCategoryNamesForCourses(List<int> courseIds, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<CategoryEntity>> SearchCategory(string searchText, PaginationDTO pagination, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
