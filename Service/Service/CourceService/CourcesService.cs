
using AutoMapper;
using Core.Common;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Interfaces.UoW;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.Courses.input;
using Core.Model.TargetDTO.Courses.output;
using infrastructure.Entitiеs;
using infrastructure.Extensions;
using infrastructure.Utils.Mapping.MapperDTO;
using infrastructure.Utils.PageService;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;


namespace Applcation.Service.CourceService
{
    public class CourcesService : ICourseService
    {
        
        private readonly IBaseRepository<CourseEntities> _courceRepository;
        
        private readonly IBaseRepository<Course_CategoriesEntities> _course_CategoriesRepository;

        private readonly ILogger<CourcesService> _logger;

        private readonly IMapper _mapper;

        private readonly IUnitOfWork _unitOfWork;

        public CourcesService(
            IBaseRepository<CourseEntities> baseRepository,
            ILogger<CourcesService> logger,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IBaseRepository<UserEntities> userRepository,
            IBaseRepository<Course_CategoriesEntities> course_CategoriesRepository
            )
        {
            _courceRepository = baseRepository;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _course_CategoriesRepository = course_CategoriesRepository;
        }



        public async Task<TResult> CreateCourse(
            CreateCourseDTO courceDTO, 
            int id,
            CancellationToken ct = default)
        {
            Console.WriteLine($"courseid : {id}");
         

            bool cource = await _courceRepository
                .GetAllWithoutTracking()
                .Where(c => c.creatorid == id && 
                       c.name == courceDTO.name)
                .AnyAsync(ct);

            if(cource)
            {
                return TResult.FailedOperation(errorCode.CourseTitleAlreadyExists);
            }

            var source = new CourcesMappingSource
            {
                CourceDTO = courceDTO,
                id = id,
                field = courceDTO.field
            };

            CourseEntities courseEntities = _mapper.Map<CourseEntities>(source);
            await _courceRepository.Create(courseEntities);

            foreach(var i in courceDTO.categoryid)
            {
                Course_CategoriesEntities course_CategoriesEntities = new Course_CategoriesEntities { course = courseEntities, categoryid = i };
                await _course_CategoriesRepository.Create(course_CategoriesEntities);
            }

            try
            {
                await _unitOfWork.CommitAsync(ct);
                return TResult.CompletedOperation();
            }
            catch(DbUpdateException ex)
            {
                _logger.LogError(ex);                                                           
                return TResult.FailedOperation(errorCode.DatabaseError);
            }

        }


        public async Task<TResult<PagedResponseDTO<CourseOutputDTO>>> SearchCourse(
            string search, 
            SortingAndPaginationDTO userSortingRequest, 
            CancellationToken ct = default)
        {
            var listqw = _courceRepository
                .GetAllWithoutTracking()
                .Where(c => c.searchvector
                .Matches(search));

            


            var list = await listqw
                .GetWithPaginationAndSorting(userSortingRequest)
                .Include(c => c.user)
                .ToListAsync(ct);
            return PageService.CreatePage(await MapList(list), userSortingRequest, await listqw.CountAsync(ct));
        }


        private async Task<List<CourseOutputDTO>> MapList(List<CourseEntities> courseEntities)
        {

            Dictionary<int, Dictionary<int, string>> categoryNames = new Dictionary<int, Dictionary<int, string>>();

            foreach (var i in courseEntities)
            {
                categoryNames = await _course_CategoriesRepository
               .GetAllWithoutTracking()
               .Where(c => c.courseid == i.id)
               .Include(c => c.categories)
               .GroupBy(c => c.courseid)
               .ToDictionaryAsync(c => c.Key, c => c.Select(c => c.categories).ToDictionary(c => c.id, c => c.name));
            }

            return courseEntities
               .Select(c => new CourseOutputDTO
               {
                   categorynames = categoryNames.ContainsKey(c.id) ? categoryNames[c.id] : new Dictionary<int, string>(),
                   field = c.field,
                   description = c.description,
                   creatorid = c.user.id,
                   id = c.id,
                   name = c.name,
                   creatorname = c.user.name ?? "удаленный аккаунт",
                   createdat = c.createdat,
               }).ToList();
        }


        public async Task<TResult<PagedResponseDTO<CourseOutputDTO>>> GetAllCourse(
            SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct = default
            )
        {
            var courses = await _courceRepository.GetAllWithoutTracking()
                .Include(c => c.user)
                .GetWithPaginationAndSorting(userSortingRequest, "courseid", "creatorid", "description")
                .ToListAsync(ct);
       
            return PageService.CreatePage(
                await MapList(courses), 
                userSortingRequest, 
                await _courceRepository.GetAll()
                    .CountAsync(ct));

        }

 
     
        public async Task<TResult<CourseOutputDTO>> UpdateCourse(
            UpdateCourseDTO updateCourseDTO, 
            int userid,
            CancellationToken ct = default)
        {
            var cousrse = await _courceRepository.GetAllWithoutTracking()
                .Where(c => c.id == updateCourseDTO.id && c.creatorid == userid)
                .FirstOrDefaultAsync(ct);

            if(cousrse == null)
            {
                return TResult<CourseOutputDTO>.FailedOperation(errorCode.CoursesNotFoud);
            }

            await _courceRepository.PartialUpdateAsync(cousrse, updateCourseDTO);

            try
            {
                await _unitOfWork.CommitAsync(ct);
                return TResult<CourseOutputDTO>.CompletedOperation(_mapper.Map<CourseOutputDTO>(updateCourseDTO));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDBError(ex);
                return TResult<CourseOutputDTO>.FailedOperation(errorCode.DatabaseError);
            }
           
        }



        public async Task<TResult<PagedResponseDTO<CourseOutputDTO>>> GetUserCourses(
            int userid, 
            string name, 
            SortingAndPaginationDTO userSortingRequest)
        {

          
            List<CourseEntities> courseEntites = await _courceRepository.GetAllWithoutTracking()
                .Include(c => c.user)
                .GetWithPaginationAndSorting(userSortingRequest, "courseid", "creatorid", "description")
                .Where(c => c.creatorid == userid)
                .ToListAsync();

            if(courseEntites == null)
            {
                return TResult<PagedResponseDTO <CourseOutputDTO>>.FailedOperation(errorCode.CoursesNotFoud, "у вас нету своих курсов");
            }

            List<CourseOutputDTO> courseDTOs = await MapList(courseEntites);

            return PageService
                .CreatePage(courseDTOs, 
                userSortingRequest, 
                await _courceRepository.GetAll()
                    .Where(c => c.creatorid == userid)
                    .CountAsync());

        }


        public async Task<TResult> RemoveCourse(
            int courseid, 
            ClaimsPrincipal userClaim,
            CancellationToken ct = default
            )
        {
            if(userClaim.IsInRole(Enum.GetName(AllRole.user)))
            {

                var cource = await _courceRepository.GetAllWithoutTracking()
                             .Include(c => c.user)
                             .Where(
                             c => c.id == courseid &&  
                             c.user.email == userClaim.FindFirst(ClaimTypes.Email).Value.ToString())
                             .FirstOrDefaultAsync(ct);

                //UserEntities? user = await _userRepository.GetAll()
                //    .Where(c => c.email == )
                //    .FirstOrDefaultAsync();
             
                if(cource == null)
                {
                    return TResult.FailedOperation(errorCode.CoursesNotFoud, "курс для удаление не найден");
                }

                await _courceRepository.DeleteById(ct, cource.id);

            }
            else if (userClaim.IsInRole(Enum.GetName(AllRole.admin)))
            {

                var course =  await _courceRepository.GetByIdAsync(ct, courseid);

                if(course != null)
                await _courceRepository.DeleteById(ct, courseid);
            }


            try
            {
                await _unitOfWork.CommitAsync(ct);
                return TResult.CompletedOperation();
            }
            catch(DbUpdateException ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.DatabaseError);

            }
        }
    }
}
