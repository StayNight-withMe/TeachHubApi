using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.Subscription.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Service
{
    public interface ISubscriptionService
    {
        Task<TResult> CreateSubscription(int followerid, int followingid);
        Task<TResult<PagedResponseDTO<SubscribeOutput>>> GetUserFollowing(int userid, SortingAndPaginationDTO userSortingRequest);
        Task<TResult<PagedResponseDTO<SubscribeOutput>>> GetUserFollowers(int userid, SortingAndPaginationDTO userSortingRequest);
        Task<TResult> DeleteSubscription(int followerid, int followingid);
    }
}
