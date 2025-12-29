using Application.Abstractions.Repository.Base;
using Ardalis.Specification;
using Core.Model.TargetDTO.Common.input;
using infrastructure.DataBase.Entitiеs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Repository.Custom
{
    public interface ICourseRepository : IBaseRepository<CourseEntity>
    {
        /// <summary>
        /// Поиск курсов используя вектор и Matches из EF
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public Task<List<CourseEntity>> SearchCourse(
            string searchText,
            ISpecification<CourseEntity> spec,
            SortingAndPaginationDTO dto,
            CancellationToken ct = default
            );
        
        public Task<int> CountofSearchCourse(
            string searchText,
            ISpecification<CourseEntity> spec,
            CancellationToken ct = default
            );


        public Task<List<CourseEntity>> GetAllCourse(
            SortingAndPaginationDTO dto,
            Specification<CourseEntity> spec,
            string[] noToSort,
            CancellationToken ct = default
            );

        public Task<List<CourseEntity>> GetUserCourse(
            int userId,
            SortingAndPaginationDTO dto,
            Specification<CourseEntity> spec,
            string[] noToSort,
            CancellationToken ct = default);

        public Task<int> GetCourseIdByUserEmailAndId(
            
            );

    }
}
