using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Interfaces.UoW;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.Subscription.output;
using infrastructure.Entitiеs;
using infrastructure.Extensions;
using infrastructure.Utils.PageService;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Threading.Tasks;

namespace Applcation.Service.SubscriptionService
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ILogger<SubscriptionService> _logger;
        
        private readonly IBaseRepository<SubscriptionEntites> _suubscriptionRepository;
        
        private readonly IUnitOfWork _unitOfWork;
        
        //private readonly IMapper _mapper;

        public SubscriptionService(
            IBaseRepository<SubscriptionEntites> subrepo,
            ILogger<SubscriptionService> logger,
            IUnitOfWork unitOfWork
            //IMapper mapper
            ) 
        {

         _logger = logger;
         //_mapper = mapper;
         _suubscriptionRepository = subrepo;
         _unitOfWork = unitOfWork;
        }

        public async Task<TResult> CreateSubscription
            (int followerid, 
            int followingid,
            CancellationToken ct = default
            )
        {
            await _suubscriptionRepository.Create(new SubscriptionEntites { followerid = followerid, followingid = followingid });
        
            try
            {
                await _unitOfWork.CommitAsync(ct);
                return TResult.CompletedOperation();
            }
   
            catch(DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
            {
                _logger.LogDBError(ex);
                if (pgEx.SqlState == 23505.ToString() || pgEx.SqlState == 23514.ToString())
                    return TResult.FailedOperation(errorCode.FollowingError);

                return TResult.FailedOperation(errorCode.DatabaseError);
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.UnknownError);
            }

        }

        public async Task<TResult> DeleteSubscription(
            int followerid, 
            int followingid,
            CancellationToken ct = default
            )
        {
            try
            {
                await _suubscriptionRepository.DeleteById(ct, followingid, followerid);
                await _unitOfWork.CommitAsync();
                return TResult.CompletedOperation();
            }
            catch(DbUpdateException ex)
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


        private async Task<TResult<PagedResponseDTO<SubscribeOutput>>> CreatePageOfUser(
            IQueryable<SubscriptionEntites> queryable,
            SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct = default
            )
        {

            var entityList = await queryable
                .Include(c => c.following)
                .Include(c => c.follower)
                .GetWithPaginationAndSorting(userSortingRequest)
                .ToListAsync(ct);


            var dtoList = entityList.Select(c => new SubscribeOutput
            {
                id = c.followingid,
                username = c.following.name,
            }
          ).ToList();

            return PageService.CreatePage(
                dtoList,
                userSortingRequest,
                await _suubscriptionRepository
                   .GetAllWithoutTracking()
                   .CountAsync());
        }


        public async Task<TResult<PagedResponseDTO<SubscribeOutput>>> GetUserFollowing(
            int userid, 
            SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct = default)
        {
            var entityqw = _suubscriptionRepository
                .GetAllWithoutTracking()
                .Where(c => c.followerid == userid);
                
            return await CreatePageOfUser(entityqw, userSortingRequest, ct);

        }

        public async Task<TResult<PagedResponseDTO<SubscribeOutput>>> GetUserFollowers(
            int userid, 
            SortingAndPaginationDTO userSortingRequest, 
            CancellationToken ct = default)
        {
            var entityqw = _suubscriptionRepository
                .GetAllWithoutTracking()
                .Where(c => c.followerid == userid);

            return await CreatePageOfUser(entityqw, userSortingRequest, ct);

        }



    }
}
