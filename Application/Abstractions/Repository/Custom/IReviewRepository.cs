using Application.Abstractions.Repository.Base;
using Ardalis.Specification;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Review.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Repository.Custom
{
    public interface IReviewRepository : IBaseRepository<ReviewEntity>
    {
        Task ExecuteDeleteBySpecAsync(
            ISpecification<ReviewEntity> spec, 
            CancellationToken ct = default);

        Task<List<ReviewOutputDTO>> GetPagedReviewsDtoAsync(
        ISpecification<ReviewEntity> spec,
        SortingAndPaginationDTO sorting,
        CancellationToken ct);

    }
}
