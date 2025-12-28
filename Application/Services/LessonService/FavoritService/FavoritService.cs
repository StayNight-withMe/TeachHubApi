using Core.Interfaces.Service;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.Favorit.output;
using infrastructure.Extensions;
//using infrastructure.Repository.Base;
using infrastructure.Utils.PageService;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces.Repository;
using infrastructure.DataBase.Entitiеs;
using Applcation.Abstractions.UoW;

namespace Applcation.Services.LessonService.FavoritService
{
    public class FavoritService : IFavoritService
    {
        private readonly ILogger<FavoritService> _logger;

        private readonly IBaseRepository<FavoritEntities> _favoritrepo;

        private readonly IUnitOfWork _unitOfWork;

        public FavoritService(
            ILogger<FavoritService> logger,
            IBaseRepository<FavoritEntities> favoritrepo,
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
            
            await _favoritrepo.Create(new FavoritEntities { userid = userid, courseid = courseid });

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

        public async Task<TResult> DeleteFavorit(int userid, int courseid,
            CancellationToken ct = default)
        {
            try
            {
                await _favoritrepo.GetAll()
                    .Where(c => c.courseid == courseid && c.userid == userid)
                    .ExecuteDeleteAsync();
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
            var entitiesList = await _favoritrepo.GetAllWithoutTracking().GetWithPaginationAndSorting(sort)
               .Include(c => c.course)
               .Include(c => c.user)
               .Where(c => c.userid == userid).ToListAsync();

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

             return PageService.CreatePage(dtoList, sort, await _favoritrepo.GetAllWithoutTracking().Where(c => c.userid == userid).CountAsync());
            

        }
    }
}
