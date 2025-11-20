
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
using infrastructure.Utils.SortBuilder;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Security.Claims;
using System.Text;


namespace Applcation.Service.CourceService
{
    public class CourcesService : ICourseService
    {

        private readonly IBaseRepository<CourseEntities> _courceRepository;

        private readonly IBaseRepository<UserEntities> _userRepository;

        private readonly ILogger<CourcesService> _logger;

        private readonly IMapper _mapper;

        private readonly IUnitOfWork _unitOfWork;

        public CourcesService(
            IBaseRepository<CourseEntities> baseRepository,
            ILogger<CourcesService> logger,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IBaseRepository<UserEntities> userRepository
            )
        {
            _courceRepository = baseRepository;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }



        public async Task<TResult> CreateCourse(CreateCourseDTO courceDTO, int id)
        {
            Console.WriteLine($"id : {id}");
         

            bool cource = await _courceRepository.GetAllWithoutTracking().Where(c => c.creatorid == id && c.name == courceDTO.name).AnyAsync();
            Console.WriteLine($"найден ли курс с таким же названием у пользователя : {cource} ");
            if(cource)
            {
                return TResult.FailedOperation(errorCode.CourseTitleAlreadyExists, "у вас уже есть курс с таким названием" );
            }

            var source = new CourcesMappingSource
            {
                CourceDTO = courceDTO,
                id = id,
                field = courceDTO.field
            };

            CourseEntities courseEntities = _mapper.Map<CourseEntities>(source);
            await _courceRepository.Create(courseEntities);

            try
            {
                await _unitOfWork.CommitAsync();
                return TResult.CompletedOperation();
            }
            catch(DbUpdateException ex)
            {
                _logger.LogError(ex);                                                           
                return TResult.FailedOperation(errorCode.DatabaseError, "ошибка на стороне сервера, \n не удалось сохранить курс");
            }

        }

        public async Task<TResult<PagedResponseDTO<CourseOutputDTO>>> GetAllCourse(UserSortingRequest userSortingRequest)
        {

            var courses = await _courceRepository.GetAllWithoutTracking().GetWithPagination(userSortingRequest)
                .ToListAsync();

           var creatorsid = courses
                .Select(c => c.creatorid).Distinct().ToList();
            var creatorsName = await _userRepository.GetAll()
                .Where(c => creatorsid.Contains(c.id)).ToDictionaryAsync(c => c.id, c => c.name);
                

            List<CourseOutputDTO> courseDTOs =  courses
                .Select( c => new CourseOutputDTO
                { description =  c.description, 
                    id = c.id.ToString(), 
                    name = c.name, 
                    creatorname = creatorsName.GetValueOrDefault(Convert.ToInt32(c.creatorid), "удаленный аккаунт"),
                      createdat = c.createdat,
                } ).ToList();


            SortBuilder<CourseOutputDTO> sortBuilder = new(courseDTOs);

            return PageService.CreatePage(
                sortBuilder, 
                userSortingRequest, 
                await _courceRepository.GetAll()
                    .CountAsync());

        }

 
     
        public async Task<TResult<CourseOutputDTO>> UpdateCourse(UpdateCourseDTO updateCourseDTO, int userid)
        {
            var cousrse = await _courceRepository.GetAllWithoutTracking().Where(c => c.id == updateCourseDTO.id && c.creatorid == userid).FirstOrDefaultAsync();

            if(cousrse == null)
            {
                return TResult<CourseOutputDTO>.FailedOperation(errorCode.CoursesNotFoud);
            }

            await _courceRepository.PartialUpdateAsync(cousrse, updateCourseDTO);

            try
            {
                await _unitOfWork.CommitAsync();
                return TResult<CourseOutputDTO>.CompletedOperation(_mapper.Map<CourseOutputDTO>(updateCourseDTO));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDBError(ex);
                return TResult<CourseOutputDTO>.FailedOperation(errorCode.DatabaseError);
            }
           
        }



        public async Task<TResult<PagedResponseDTO<CourseOutputDTO>>> GetUserCourses(int userid, string name, UserSortingRequest userSortingRequest)
        {

          
            List<CourseEntities> courseEntites = await _courceRepository.GetAllWithoutTracking()
                .GetWithPagination(userSortingRequest)
                .Where(c => c.creatorid == userid)
                .ToListAsync();

            if(courseEntites == null)
            {
                return TResult<PagedResponseDTO <CourseOutputDTO>>.FailedOperation(errorCode.CoursesNotFoud, "у вас нету своих курсов");
            }

            List<CourseOutputDTO> courseDTOs = courseEntites
                .Select(c => new CourseOutputDTO
            {
                id = c.id.ToString(),

                name = c.name,

                description = c.description,

                creatorname = name,

                createdat = c.createdat,

                field = c.field
            }).ToList();

            SortBuilder<CourseOutputDTO> sortBuilder = new(courseDTOs);
           
            return PageService
                .CreatePage(sortBuilder, 
                userSortingRequest, 
                await _courceRepository.GetAll()
                    .Where(c => c.creatorid == userid)
                    .CountAsync());

        }


        public async Task<TResult> RemoveCourse(int id, ClaimsPrincipal userClaim)
        {
            if(userClaim.IsInRole(Enum.GetName(AllRole.user)))
            {
                UserEntities? user = await _userRepository.GetAll()
                    .Where(c => c.email == userClaim.FindFirst(ClaimTypes.Email).Value.ToString())
                    .FirstOrDefaultAsync();

                if(user == null)
                {
                    return TResult.FailedOperation(errorCode.EmailInvalid, "ошибка поиска пользователя");
                }

                var cource = await _courceRepository.GetAll()
                                .Where(c => c.id == id && c.creatorid == user.id)
                                .FirstOrDefaultAsync();
                
                if(cource == null)
                {
                    return TResult.FailedOperation(errorCode.CoursesNotFoud, "курс для удаление не найден");
                }

                await _courceRepository.DeleteById(cource.id);

            }
            else if (userClaim.IsInRole(Enum.GetName(AllRole.admin)))
            {

                var course =  await _courceRepository.GetById(id);

                if(course != null)
                await _courceRepository.DeleteById(id);
            }


            try
            {
                await _unitOfWork.CommitAsync();
                return TResult.CompletedOperation();
            }
            catch(DbUpdateException ex)
            {
                _logger.LogError(ex);

                return TResult.FailedOperation(errorCode.DatabaseError, "ошибка удаления курса");

            }
        }
    }
}
