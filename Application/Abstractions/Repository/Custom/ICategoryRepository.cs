using Application.Abstractions.Repository.Base;
using infrastructure.DataBase.Entitiеs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Repository.Custom
{
    public interface ICategoryRepository : IBaseRepository<Course_CategoriesEntities>
    {
        public Task<Dictionary<int, Dictionary<int, string>>> GetCategoryNamesForCourses(
            List<int> courseIds,
            CancellationToken ct = default);
            
        
        public Task<List<CategoryEntity>>

    }
}
