
using Application.Abstractions.Repository.Base;
using Application.Abstractions.Repository.Custom;
using Application.Abstractions.Service;
using Application.Abstractions.UoW;
using Application.Abstractions.Utils;
using Application.Mapping.MapperDTO;
using Application.Utils.PageService;
using Ardalis.Specification;
using AutoMapper;
using Core.Common.EnumS;
using Core.Common.Exeptions;
using Core.Common.Types.HashId;
using Core.Models.Entitiеs;
using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;
using Core.Models.TargetDTO.Courses.input;
using Core.Models.TargetDTO.Courses.output;
using Core.Specification.Common;
using Core.Specification.FavoriteSpec;
using Logger;
using Microsoft.Extensions.Logging;
using Core.Specification.CourseSpec;

namespace Application.Services.CourceService
{
    public class CourcesService : ICourseService
    {
        
        private readonly ICourseRepository _courceRepository;
        
        private readonly ICategoryRepository _course_CategoriesRepository;

        private readonly IBaseRepository<FavoritEntity> _favoriteRepository;

        private readonly ILogger<CourcesService> _logger;

        private readonly ICourseImageService _courseFileUtil;

        private readonly IMapper _mapper;

        private readonly IUnitOfWork _unitOfWork;

        public CourcesService(
            ICourseRepository baseRepository,
            IBaseRepository<FavoritEntity> favoritRepository,
            IBaseRepository<UserEntity> userRepository,
            ICategoryRepository course_CategoriesRepository,
            ILogger<CourcesService> logger,
            ICourseImageService courseFileService,
            IUnitOfWork unitOfWork,
            IMapper mapper

            )
        {
            _courceRepository = baseRepository;
            _favoriteRepository = favoritRepository;
            _course_CategoriesRepository = course_CategoriesRepository;
            _logger = logger;
            _courseFileUtil = courseFileService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }



        public async Task<TResult> CreateCourse(
            CreateCourseDTO courceDTO, 
            int id,
            CancellationToken ct = default)
        {
           
            bool exists = await _courceRepository
                .AnyAsync(new ExistsCourseSpecification(id, courceDTO.name));

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
                Course_CategoriesEntity course_CategoriesEntities = new Course_CategoriesEntity { course = courseEntities, categoryid = i };
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
            var listqw = await _courceRepository.SearchCourse(
                search, 
                new AnySpecification<CourseEntity>(),  
                userSortingRequest,
                ct);

            var count = await _courceRepository.CountofSearchCourse(
                search,
                new AnySpecification<CourseEntity>(),
                ct);

            return PageService.CreatePage(
                await MapList(listqw, userid), 
                userSortingRequest, 
                count);
        }


        private async Task<List<CourseOutputDTO>> MapList(
            List<CourseEntity> courseEntities,
            int userid = default,
            CancellationToken ct = default
            )
        {

            //Dictionary<Hashid, Dictionary<Hashid, string>> categoryNames = new();


            var courseIds = courseEntities.Select(c => c.id).ToList();

            var dbResult = await _course_CategoriesRepository
                    .GetCategoryNamesForCourses(courseIds, ct);

                    //.GetAllWithoutTracking()
                    //.Where(c => c.courseid == i.id)
                    //.Include(c => c.categories)
                    //.GroupBy(c => c.courseid)
                    //.ToDictionaryAsync(
                    //    g => (Hashid)g.Key, 
                    //    g => g.Select(c => c.categories)
                    //          .ToDictionary(cat => (Hashid)cat.id, cat => cat.name) 
                    //);

            Dictionary<int, bool> favorietDict = new();

            if (userid != default)
            {
                var favSpec = new FavoriteCoursesCountainSpec(userid, courseIds);
                favorietDict = (await _favoriteRepository.ListAsync(favSpec))
                    .ToDictionary(f => f.courseid, _ => true);

            }


            var categoryNames = dbResult.ToDictionary(
            courseEntry => (Hashid)courseEntry.Key, 
            courseEntry => courseEntry.Value.ToDictionary(
                catEntry => (Hashid)catEntry.Key,    
                catEntry => catEntry.Value           
                )
            );

            return courseEntities
               .Select(c => new CourseOutputDTO
               {
                   categorynames = categoryNames.ContainsKey(c.id) ? categoryNames[c.id] : new Dictionary<Hashid, string>(),
                   iconurl = c.imgfilekey == null ? null : _courseFileUtil.GetPresignedUrl(c.imgfilekey, 10080),
                   field = c.field,
                   description = c.description,
                   creatorid = c.user.id,
                   favorite = favorietDict.TryGetValue(c.id, out var value),
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
            var courses = await _courceRepository.GetAllCourse(userSortingRequest,
                new AnySpecification<CourseEntity>(),
                [ "userSortingRequest", "courseid", "creatorid", "description", "field" ],
                ct);


            return PageService.CreatePage(
                await MapList(courses),
                userSortingRequest,
                await _courceRepository.CountAsync(new AnySpecification<CourseEntity>(), ct) );
        }

 
     
        public async Task<TResult<CourseOutputDTO>> UpdateCourse(
            UpdateCourseDTO updateCourseDTO, 
            int userid,
            CancellationToken ct = default)
        {
            var cousrse = await _courceRepository
                .FirstOrDefaultAsync(new UserCourseSpecification(userid, updateCourseDTO.id), ct);

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
            int userId, 
            SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct = default
            )
        {


            var courseEntites = await _courceRepository.GetUserCourseId(
                userId,
                userSortingRequest,
                new AnySpecification<CourseEntity>(),
                ["userSortingRequest", "courseid", "creatorid", "description"], 
                ct );


            if(courseEntites == null)
            {
                return TResult<PagedResponseDTO <CourseOutputDTO>>
                    .FailedOperation(errorCode.CoursesNotFoud);
            }

            List<CourseOutputDTO> courseDTOs = await MapList(courseEntites);

            return PageService
                .CreatePage(courseDTOs, 
                userSortingRequest, 
                await _courceRepository.CountAsync(new UserCourseSpecification(userId)));

        }


        public async Task<TResult> RemoveCourse(
            int courseid,
            int userId,
            AllRole role,
            CancellationToken ct = default
            )
        {
            if (role == AllRole.user)
            {

                var courceid = await _courceRepository
                    .FirstOrDefaultAsync(new UserCourseIdSpecification(userId, courseid));

                if (courceid == default ||
                    courceid == 0 
                    )
                {
                    return TResult.FailedOperation(errorCode.CoursesNotFoud);
                }

                await _courceRepository.DeleteById(ct, courceid);

            }
            else if (role == AllRole.admin )
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
            var course = await _courceRepository.FirstOrDefaultAsync(
                new UserCourseSpecification(userid, courseSetImageDTO.courseid, true));

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

                                await _courseFileUtil.DeleteFileAsync(
                              course.imgfilekey,
                              ct);

                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex);
                                return TResult<SetImageOutPutDTO>.FailedOperation(errorCode.UnknownError);
                            }

                        }

                        string fileKey = await _courseFileUtil.UploadFileAsync(
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
                                 new SetImageOutPutDTO{ iconusrl = _courseFileUtil.GetPresignedUrl(fileKey, 60*24) }
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

