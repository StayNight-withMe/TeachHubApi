using Application.Abstractions.Repository.Base;
using Ardalis.Specification;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Common.input;


namespace Application.Abstractions.Repository.Custom
{
    public interface IChapterRepository : IBaseRepository<ChapterEntity>
    {
        Task<List<ChapterEntity>> GetPagedChaptersAsync(
            ISpecification<ChapterEntity> spec,
            SortingAndPaginationDTO sorting,
            CancellationToken ct = default);

        Task<int> ExecuteDeleteBySpecAsync(
            ISpecification<ChapterEntity> spec, 
            CancellationToken ct = default);
    }
}
