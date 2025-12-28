
using Amazon.S3;
using Applcation.Abstractions.Service;
using Applcation.Abstractions.UoW;
using AutoMapper;
using Core.Common.EnumS;
using Core.Common.Types.HashId;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.Courses.input;
using Core.Model.TargetDTO.Courses.output;
using Core.Specification.CourseSpecification;
using infrastructure.DataBase.Entitiеs;
using infrastructure.Extensions;
using infrastructure.Utils.Mapping.MapperDTO;
using infrastructure.Utils.PageService;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;


namespace Applcation.Services.CourceService
{
    public class CourcesService : ICourseService
    {
        
        private readonly IBaseRepository<CourseEntity> _courceRepository;
        
        private readonly IBaseRepository<Course_CategoriesEntities> _course_CategoriesRepository;

        private readonly IBaseRepository<FavoritEntities> _favoriteRepository;

        private readonly ILogger<CourcesService> _logger;

        private readonly ICourseImageService _courseFileService;

        //private readonly IMapper _mapper;

        private readonly IUnitOfWork _unitOfWork;

        public CourcesService(
            IBaseRepository<CourseEntity> baseRepository,
            IBaseRepository<FavoritEntities> favoritRepository,
            IBaseRepository<UserEntity> userRepository,
            IBaseRepository<Course_CategoriesEntities> course_CategoriesRepository,
            ILogger<CourcesService> logger,
            ICourseImageService courseFileService,
            IUnitOfWork unitOfWork,
            //IMapper mapper
            
            )
        {
            _courceRepository = baseRepository;
            _favoriteRepository = favoritRepository;
            _course_CategoriesRepository = course_CategoriesRepository;
            _logger = logger;
            _courseFileService = courseFileService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }



        public async Task<TResult> CreateCourse(
            CreateCourseDTO courceDTO, 
            int id,
            CancellationToken ct = default)
        {
           


            bool exists = await _courceRepository
                .AnyAsync(new ExistsSpecification(id, courceDTO.name));

            if(exists)
            {
                return TResult.FailedOperation(errorCode.CourseTitleAlreadyExists);
            }

            var source = new CourcesMappingSource
            {
                CourceDTO = courceDTO,
                id = id,
                field = courceDTO.field
            };

            CourseEntity courseEntities = _mapper.Map<CourseEntity>(source);

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
            int userid = default,
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

            return PageService.CreatePage(await MapList(list, userid), userSortingRequest, await listqw.CountAsync(ct));
        }


        private async Task<List<CourseOutputDTO>> MapList(
            List<CourseEntity> courseEntities,
            int userid = default
            )
        {

            Dictionary<Hashid, Dictionary<Hashid, string>> categoryNames = new();

            foreach (var i in courseEntities)
            {
                // Получаем данные из БД (там int)
                var dbResult = await _course_CategoriesRepository
                    .GetAllWithoutTracking()
                    .Where(c => c.courseid == i.id)
                    .Include(c => c.categories)
                    .GroupBy(c => c.courseid)
                    .ToDictionaryAsync(
                        g => (Hashid)g.Key, // Преобразуем ID курса в Hashid
                        g => g.Select(c => c.categories)
                              .ToDictionary(cat => (Hashid)cat.id, cat => cat.name) // Преобразуем ID категории в Hashid
                    );

                // Добавляем в общий словарь
                foreach (var entry in dbResult)
                {
                    categoryNames[entry.Key] = entry.Value;
                }
            }





            Dictionary<int, bool> favorietdict = new();

            if (userid != default)
            {
                var userCourseId =  courseEntities
                .Where(c => c.creatorid == userid)
                .Select(c => c.id)
                .FirstOrDefault();

                favorietdict = await _favoriteRepository
                    .GetAllWithoutTracking()
                    .Where(c => c.userid == userid && c.courseid == userCourseId)
                    .ToDictionaryAsync(c => c.courseid, c => true);

            }




            return courseEntities
               .Select(c => new CourseOutputDTO
               {
                   categorynames = categoryNames.ContainsKey(c.id) ? categoryNames[c.id] : new Dictionary<Hashid, string>(),
                   iconurl = c.imgfilekey == null ? null : _courseFileService.GetPresignedUrl(c.imgfilekey, 10080),
                   field = c.field,
                   description = c.description,
                   creatorid = c.user.id,
                   favorite = favorietdict.TryGetValue(c.id, out var value),
                   id = c.id,
                   name = c.name,
                   creatorname = c.user.name ?? "удаленный аккаунт",
                   createdat = c.createdat,
                   rating = c.rating
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
            SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct = default
            )
        {

          
            List<CourseEntity> courseEntites = await _courceRepository.GetAllWithoutTracking()
                .Include(c => c.user)
                .GetWithPaginationAndSorting(userSortingRequest, "courseid", "creatorid", "description")
                .Where(c => c.creatorid == userid)
                .ToListAsync(ct);

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
                    .CountAsync(ct));

        }


        public async Task<TResult> RemoveCourse(
            int courseid, 
            ClaimsPrincipal userClaim,
            CancellationToken ct = default
            )
        {
            if(userClaim.IsInRole(Enum.GetName(AllRole.user)))
            {

                var cource = await _courceRepository
                            .GetAllWithoutTracking()
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
            catch(Exception ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.DatabaseError);

            }
        }

        public async Task<TResult<SetImageOutPutDTO>> SetImgFile(
            Stream? stream, 
            int userid, 
            CourseSetImageDTO courseSetImageDTO,
            string? ContentType,
            CancellationToken ct = default)
        {
            var course = await _courceRepository
                .GetAll()
                .Where(c => c.id == courseSetImageDTO.courseid && 
                       c.creatorid == userid)
                .FirstOrDefaultAsync(ct);

            switch(courseSetImageDTO.setstatus)
            {
                case SetImageStatus.Upload:
                    {
                        if(stream == null)
                        {
                            return TResult<SetImageOutPutDTO>.FailedOperation(errorCode.InvalidDataFormat);
                        }


                        _logger.LogInformation("Upload");
                        if (course == null)
                        {
                            return TResult<SetImageOutPutDTO>.FailedOperation(errorCode.CoursesNotFoud);
                        }




                        if (!string.IsNullOrEmpty(course.imgfilekey))
                        {
                            try
                            {

                                await _courseFileService.DeleteFileAsync(
                              course.imgfilekey,
                              ct);

                            }
                            catch (AmazonS3Exception ex)
                            {
                                _logger.LogError(ex);
                                return TResult<SetImageOutPutDTO>.FailedOperation(errorCode.CloudError);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex);
                                return TResult<SetImageOutPutDTO>.FailedOperation(errorCode.UnknownError);
                            }

                        }

                        string fileKey = await _courseFileService.UploadFileAsync(
                            stream,
                            course.id,
                            ContentType,
                            "courseimg",
                            ct);
                        course.imgfilekey = fileKey;

                        try
                        {
                            await _unitOfWork.CommitAsync(ct);
                            return TResult<SetImageOutPutDTO>.CompletedOperation(
                                 new SetImageOutPutDTO{ iconusrl = _courseFileService.GetPresignedUrl(fileKey, 60*24) }
                                );
                        }
                        catch (DbUpdateException ex)
                        {
                            _logger.LogDBError(ex);
                            return TResult<SetImageOutPutDTO>.FailedOperation(errorCode.DatabaseError);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex);
                            return TResult<SetImageOutPutDTO>.FailedOperation(errorCode.UnknownError);
                        }
                    }

                case SetImageStatus.Remove:
                    {
                        _logger.LogInformation("Remove");
                        var course1 = await _courceRepository
                         .GetAll()
                         .Where(c => c.id == courseSetImageDTO.courseid &&
                                c.creatorid == userid)
                         .FirstOrDefaultAsync(ct);

                        course.imgfilekey = null;

                        try
                        {
                            var count = await _unitOfWork.CommitAsync(ct);
                            if(count == 0)
                            {
                                return TResult<SetImageOutPutDTO>.FailedOperation(errorCode.NotFound);
                            }
                            return TResult<SetImageOutPutDTO>.CompletedOperation(new SetImageOutPutDTO());
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex);
                            return TResult<SetImageOutPutDTO>.FailedOperation(errorCode.UnknownError);
                        }
                    }

                default:
                    return TResult<SetImageOutPutDTO>.FailedOperation(errorCode.InvalidDataFormat);
                        

                    }




            }

        }
    }

