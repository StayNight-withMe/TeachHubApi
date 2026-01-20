using Application.Abstractions.Repository.Custom;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Review.output;
using infrastructure.DataBase.Context;
using infrastructure.DataBase.Extensions;
using infrastructure.DataBase.Repository.Base;
using Microsoft.EntityFrameworkCore;

public class ReviewRepository : BaseRepository<ReviewEntity>, IReviewRepository
{
    public ReviewRepository(CourceDbContext dbContext) : base(dbContext) { }

    public async Task ExecuteDeleteBySpecAsync(
        ISpecification<ReviewEntity> spec,
        CancellationToken ct = default)
    {
        var query = ApplySpecification(spec);
        await query.ExecuteDeleteAsync(ct);
    }


    public async Task<List<ReviewOutputDTO>> GetPagedReviewsDtoAsync(
            ISpecification<ReviewEntity> spec,
            SortingAndPaginationDTO sorting,
            CancellationToken ct)
    {

        var query = ApplySpecification(spec);
        return await query
            .GetWithPaginationAndSorting(sorting)
            .Select(c => new ReviewOutputDTO
            {
                id = c.id,
                content = c.content,
                createdat = c.createdat,
                courseId = c.courseid,
                review = c.review,
                userId = c.userid,
                dislikecount = c.dislikecount,
                likecount = c.likecount,
                lastchangedat = c.lastchangedat
            })
            .ToListAsync(ct);
    }
}