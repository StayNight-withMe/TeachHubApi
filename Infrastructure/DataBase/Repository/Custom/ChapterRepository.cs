using Application.Abstractions.Repository.Custom;
using Ardalis.Specification;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Common.input;
using infrastructure.DataBase.Context;
using infrastructure.DataBase.Extensions;
using infrastructure.DataBase.Repository.Base;
using Microsoft.EntityFrameworkCore;


namespace infrastructure.DataBase.Repository.Custom
{
    public class ChapterRepository : BaseRepository<ChapterEntity>, IChapterRepository
    {
        public ChapterRepository(CourceDbContext dbContext) : base(dbContext) { }

        public async Task<List<ChapterEntity>> GetPagedChaptersAsync(
            ISpecification<ChapterEntity> spec,
            SortingAndPaginationDTO sorting,
            CancellationToken ct)
        {
            var query = ApplySpecification(spec); 

            return await query
                .GetWithPaginationAndSorting(sorting, "id", "courseid")
                .ToListAsync(ct);
        }

        public async Task<int> ExecuteDeleteBySpecAsync(
            ISpecification<ChapterEntity> spec, 
            CancellationToken ct = default)
        {
            
            var query = ApplySpecification(spec);

            return await query.ExecuteDeleteAsync(ct);
        }
    }
}
