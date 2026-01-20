using Application.Abstractions.Repository.Base;
using Ardalis.Specification;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Subscription.output;


namespace Application.Abstractions.Repository.Custom
{
    public interface ISubscriptionRepository : IBaseRepository<SubscriptionEntites>
    {
        Task<List<SubscribeOutput>> GetPagedSubscriptionsDtoAsync(
            ISpecification<SubscriptionEntites> spec,
            SortingAndPaginationDTO sorting,
            CancellationToken ct);

        Task<List<SubscribeOutput>> GetPagedFollowersDtoAsync(
            ISpecification<SubscriptionEntites> spec,
            SortingAndPaginationDTO sorting,
            CancellationToken ct);
    }
}
