using Applcation.Abstractions.Repository.Base;
using Core.Model.TargetDTO.Common.input;
using infrastructure.DataBase.Entitiеs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applcation.Abstractions.Repository.Custom
{
    public interface ICourseRepository : IBaseRepository<CourseEntity>
    {
        /// <summary>
        /// Поиск курсов используя вектор и Matches из EF
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public Task<List<CourseEntity>> SearchCourse(string searchText, SortingAndPaginationDTO dto);
        
        public Task<int> CountofSearchCourse(string searchText);
    }
}
