using AutoMapper;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Interfaces.UoW;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.Subscription.output;
using infrastructure.Entitiеs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using infrastructure.Extensions;
using infrastructure.Utils.PageService;
using Logger;

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

        public async Task<TResult> CreateSubscription(int followerid, int followingid)
        {
            await _suubscriptionRepository.Create(new SubscriptionEntites { followerid = followerid, followingid = followingid });
        
            try
            {
                await _unitOfWork.CommitAsync();
                return TResult.CompletedOperation();
            }
            catch(DbUpdateException ex)
            {
                _logger.LogDBError(ex);
                return TResult.FailedOperation(errorCode.DatabaseError);
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.UnknownError);
            }

        }

        public async Task<TResult> DeleteSubscription(int followerid, int followingid)
        {
            try
            {
                await _suubscriptionRepository.GetAll().Where(c => c.followingid == followingid && c.followerid == followerid).ExecuteDeleteAsync();
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

        public async Task<TResult<PagedResponseDTO<SubscribeOutput>>> GetSubscriptionOfUser(int userid, UserSortingRequest userSortingRequest)
        {
            var entityList =  await _suubscriptionRepository.GetAllWithoutTracking().Include(c => c.following).GetWithPaginationAndSorting(userSortingRequest).Where(c => c.followerid == userid).ToListAsync();

            var dtoList = entityList.Select(c => new SubscribeOutput
            {
                id = c.followingid,
                username = c.following.name,
            }
            ).ToList();

             return PageService.CreatePage(dtoList, userSortingRequest, await _suubscriptionRepository.GetAllWithoutTracking().CountAsync());

        }


    }
}
