using Application.Abstractions.Repository.Base;
using Ardalis.Specification;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Common.input;
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
        
        public Task<int> CountOfSearchCourse(
            string searchText,
            ISpecification<CourseEntity> spec,
            CancellationToken ct = default
            );


        public Task<List<CourseEntity>> GetAllCourse( 
            SortingAndPaginationDTO dto,
            Specification<CourseEntity> spec,
            string[]? noToSort,
            CancellationToken ct = default
            );

        public Task<List<CourseEntity>> GetUserCourses(
            int userId,
            SortingAndPaginationDTO? dto,
            Specification<CourseEntity> spec,
            string[]? noToSort,
            CancellationToken ct = default);


        //public Task<List<CourseEntity>> GetUserCourseId(
        //int userId,
        //SortingAndPaginationDTO? dto,
        //Specification<CourseEntity> spec,
        //string[]? noToSort,
        //CancellationToken ct = default);


    }
}
