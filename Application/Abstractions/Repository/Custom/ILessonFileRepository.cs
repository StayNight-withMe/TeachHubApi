using Application.Abstractions.Repository.Base;
using Ardalis.Specification;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Common.input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Repository.Custom
{
    public interface ILessonFileRepository : IBaseRepository<LessonfileEntity>
    {
        Task<List<LessonfileEntity>> GetPagedLessonFilesAsync(
            ISpecification<LessonfileEntity> spec,
            PaginationDTO pagination,
            CancellationToken ct);
    }
}
