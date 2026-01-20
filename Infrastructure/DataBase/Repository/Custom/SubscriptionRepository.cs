using Application.Abstractions.Repository.Custom;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Subscription.output;
using infrastructure.DataBase.Context;
using infrastructure.DataBase.Extensions;
using infrastructure.DataBase.Repository.Base;
using Microsoft.EntityFrameworkCore;

public class SubscriptionRepository : BaseRepository<SubscriptionEntites>, ISubscriptionRepository
{
    public SubscriptionRepository(CourceDbContext dbContext) : base(dbContext) { }

    public async Task<List<SubscribeOutput>> GetPagedSubscriptionsDtoAsync(
        ISpecification<SubscriptionEntites> spec,
        SortingAndPaginationDTO sorting,
        CancellationToken ct)
    {
        var query = ApplySpecification(spec);

        return await query
            .GetWithPaginationAndSorting(sorting)
            .Select(s => new SubscribeOutput
            {
                id = s.followingid,
                username = s.following.name 
            })
            .ToListAsync(ct);
    }

    public async Task<List<SubscribeOutput>> GetPagedFollowersDtoAsync(
        ISpecification<SubscriptionEntites> spec,
        SortingAndPaginationDTO sorting,
        CancellationToken ct)
    {
        var query = ApplySpecification(spec);

        return await query
            .GetWithPaginationAndSorting(sorting)
            .Select(s => new SubscribeOutput
            {
                id = s.followerid,
                username = s.follower.name
            })
            .ToListAsync(ct);
    }
}