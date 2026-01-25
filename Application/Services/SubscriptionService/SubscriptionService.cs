using Application.Abstractions.Repository.Custom;
using Application.Abstractions.Service;
using Application.Abstractions.UoW;
using Application.Utils.PageService;
using Core.Common.Exeptions;
using Core.Models.Entitiеs;
using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;
using Core.Models.TargetDTO.Subscription.output;
using Core.Specification.SubscriptionsSpec;
using Logger;
using Microsoft.Extensions.Logging;

namespace Application.Services.SubscriptionService
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ILogger<SubscriptionService> _logger;
        
        private readonly ISubscriptionRepository _suubscriptionRepository;
        
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionService(
            ISubscriptionRepository subrepo,
            ILogger<SubscriptionService> logger,
            IUnitOfWork unitOfWork
            ) 
        {

         _logger = logger;
         _suubscriptionRepository = subrepo;
         _unitOfWork = unitOfWork;
        }

        public async Task<TResult> CreateSubscription
            (int followerid, 
            int followingid,
            CancellationToken ct = default
            )
        {

            await _suubscriptionRepository.Create(new SubscriptionEntity { followerid = followerid, followingid = followingid });
        
            try
            {
                await _unitOfWork.CommitAsync(ct);
                return TResult.CompletedOperation();
            }
   
            catch(DbUpdateException ex) 
            {
                _logger.LogDBError(ex);
                if (ex.ErrorCode == 23505.ToString() || ex.ErrorCode == 23514.ToString())
                    return TResult.FailedOperation(errorCode.FollowingError);

                if(ex.ErrorCode == 23503.ToString())
                    return TResult.FailedOperation(errorCode.UserNotFound);

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

                if (ex.ErrorCode == 23503.ToString())
                    return TResult.FailedOperation(errorCode.UserNotFound);

                _logger.LogDBError(ex);
                return TResult.FailedOperation(errorCode.DatabaseError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.UnknownError);
            }
        }

        //private async Task<TResult<PagedResponseDTO<SubscribeOutput>>> CreatePageOfUser(
        //    IQueryable<SubscriptionEntites> queryable,
        //    SortingAndPaginationDTO userSortingRequest,
        //    CancellationToken ct = default
        //    )
        //{

        //    var entityList = await queryable
        //        .Include(c => c.following)
        //        .Include(c => c.follower)
        //        .GetWithPaginationAndSorting(userSortingRequest)
        //        .ToListAsync(ct);


        //    var dtoList = entityList.Select(c => new SubscribeOutput
        //    {
        //        id = c.followingid,
        //        username = c.following.name,
        //    }
        //  ).ToList();

        //    return PageService.CreatePage(
        //        dtoList,
        //        userSortingRequest,
        //        await _suubscriptionRepository
        //           .GetAllWithoutTracking()
        //           .CountAsync());
        //}


        public async Task<TResult<PagedResponseDTO<SubscribeOutput>>> GetUserFollowing(
            int userid,
            SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct = default)
        {
    
            var spec = new UserFollowingSpec(userid);

            var dtoList = await _suubscriptionRepository.GetPagedSubscriptionsDtoAsync(spec, userSortingRequest, ct);

            var totalCount = await _suubscriptionRepository.CountAsync(spec, ct);

            return PageService.CreatePage(dtoList, userSortingRequest, totalCount);
        }

        public async Task<TResult<PagedResponseDTO<SubscribeOutput>>> GetUserFollowers(
            int userid,
            SortingAndPaginationDTO userSortingRequest,
            CancellationToken ct = default)
        {
            var spec = new UserFollowersSpec(userid);

            var dtoList = await _suubscriptionRepository.GetPagedFollowersDtoAsync(spec, userSortingRequest, ct);

            var totalCount = await _suubscriptionRepository.CountAsync(spec, ct);

            return PageService.CreatePage(dtoList, userSortingRequest, totalCount);
        }

    }
}
