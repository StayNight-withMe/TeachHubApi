using Application.Abstractions.Repository.Custom;
using Ardalis.Specification;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Common.input;
using infrastructure.DataBase.Context;
using infrastructure.DataBase.Extensions;
using infrastructure.DataBase.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.DataBase.Repository.Custom
{
    public class LessonFileRepository : BaseRepository<LessonfileEntity>, ILessonFileRepository
    {
        public LessonFileRepository(CourceDbContext dbContext) : base(dbContext) { }

        public async Task<List<LessonfileEntity>> GetPagedLessonFilesAsync(
            ISpecification<LessonfileEntity> spec,
            PaginationDTO pagination,
            CancellationToken ct)
        {

            var query = ApplySpecification(spec);


            return await query
                .GetWithPagination(pagination)
                .ToListAsync(ct);
        }
    }
}
