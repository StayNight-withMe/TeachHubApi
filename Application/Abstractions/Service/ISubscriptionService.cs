using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;
using Core.Models.TargetDTO.Subscription.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Service
{
    public interface ISubscriptionService
    {
        Task<TResult> CreateSubscription(
            int followerid, 
            int followingid, 
            CancellationToken ct = default);
        Task<TResult<PagedResponseDTO<SubscribeOutput>>> GetUserFollowing(
            int userid, 
            SortingAndPaginationDTO userSortingRequest, 
            CancellationToken ct = default);
        Task<TResult<PagedResponseDTO<SubscribeOutput>>> GetUserFollowers(
            int userid, SortingAndPaginationDTO userSortingRequest, 
            CancellationToken ct = default);
        Task<TResult> DeleteSubscription(
            int followerid, 
            int followingid, 
            CancellationToken ct = default);
    }
}
