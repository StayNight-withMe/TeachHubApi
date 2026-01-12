using Core.Common.Exeptions;
using Logger;
using Microsoft.Extensions.Logging;
using Application.Abstractions.UoW;
using Application.Abstractions.Repository.Base;
using Application.Abstractions.Service;
using Application.Utils.PageService;
using Core.Specification.FavoriteSpec;
using Core.Models.TargetDTO.Favorit.output;
using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;
using Core.Models.Entitiеs;

namespace Application.Services.FavoritService
{
    public class FavoritService : IFavoritService
    {
        private readonly ILogger<FavoritService> _logger;

        private readonly IBaseRepository<FavoritEntity> _favoritrepo;

        private readonly IUnitOfWork _unitOfWork;

        public FavoritService(
            ILogger<FavoritService> logger,
            IBaseRepository<FavoritEntity> favoritrepo,
            IUnitOfWork unitOfWork
            ) 
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _favoritrepo = favoritrepo;
        }

        public async Task<TResult> CreateFavorite(
            int userid, 
            int courseid,
            CancellationToken ct = default)
        {
            
            await _favoritrepo.Create(new FavoritEntity { userid = userid, courseid = courseid });

            try
            {
                await _unitOfWork.CommitAsync();
                return TResult.CompletedOperation();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDBError(ex);
                return TResult.FailedOperation(errorCode.DatabaseError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.UnknownError);
            }

        }

        public async Task<TResult> DeleteFavorit(
            int userid, 
            int courseid,
            CancellationToken ct = default)
        {

            var entity = await _favoritrepo.AnyAsync(new UsersFavoriteSpec(courseid, userid), ct);

            if (!entity)
            {
                return TResult.FailedOperation(errorCode.NotFound);
            }

            await _favoritrepo.DeleteById(
            ct,
            userid,
            courseid);

            try
            {
                await _unitOfWork.CommitAsync();
                return TResult.CompletedOperation();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDBError(ex);
                return TResult.FailedOperation(errorCode.DatabaseError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.UnknownError);
            }
        }

        public async Task<TResult<PagedResponseDTO<FavoritOutputDTO>>> GetFavorite(
            int userid, 
            SortingAndPaginationDTO sort,
            CancellationToken ct = default
            )
        {
            var entitiesList = await
                _favoritrepo.ListAsync(new UserFavorite(userid, true));
            //.GetAllWithoutTracking().GetWithPaginationAndSorting(sort)


            var dtoList = entitiesList
                .Select(c =>
                new FavoritOutputDTO
                {
                    courseid = c.courseid,
                    coursename = c.course.name,
                    creatorname = c.user.name,
                    field = c.course.field
                }
                ).ToList();

             return PageService.CreatePage(
                 dtoList, 
                 sort, 
                 await _favoritrepo.CountAsync(new UserFavorite(userid, false )));
            

        }
    }
}
