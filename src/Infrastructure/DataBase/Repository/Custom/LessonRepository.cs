using Application.Abstractions.Repository.Custom;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Lesson.output;
using infrastructure.DataBase.Context;
using infrastructure.DataBase.Extensions;
using infrastructure.DataBase.Repository.Base;
using Microsoft.EntityFrameworkCore;

public class LessonRepository : BaseRepository<LessonEntity>, ILessonRepository
{
    public LessonRepository(CourceDbContext courceDbContext) : base(courceDbContext) { }

    public async Task<List<lessonOutputDTO>> GetLessonByChapterid(
        int chapterid,
        SortingAndPaginationDTO userSortingRequest,
        ISpecification<LessonEntity> spec,
        CancellationToken ct = default)
    {

        var query = ApplySpecification(spec);

        return await query
            //.Where(c => c.chapterid == chapterid)
            .GetWithPaginationAndSorting(userSortingRequest, "isvisible", "chapterid", "id")
            .Select(c => new lessonOutputDTO
            {
                id = c.id,
                name = c.name,
                order = (int)c.order
            })
            .ToListAsync(ct);
    }


    public async Task<List<LessonUserOutputDTO>> GetLessonUserByChapterid(
        int chapterid,
        SortingAndPaginationDTO userSortingRequest,
        ISpecification<LessonEntity> spec,
        CancellationToken ct = default)
    {
        var query = ApplySpecification(spec);

        return await query
            .Where(c => c.chapterid == chapterid)
            .GetWithPaginationAndSorting(userSortingRequest, "isvisible", "chapterid", "id")
            .Select(c => new LessonUserOutputDTO
            {
                id = c.id,
                name = c.name,
                order = (int)c.order,
                isvisible = c.isvisible 
            })
            .ToListAsync(ct);
    }
}