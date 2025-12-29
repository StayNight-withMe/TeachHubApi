using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applcation.Abstractions.Repository.Custom
{
    public interface ICategoryRepository
    {
        public Task<Dictionary<int, Dictionary<int, string>>> GetCategoryNamesForCourses(List<int> courseIds);
    }
}
