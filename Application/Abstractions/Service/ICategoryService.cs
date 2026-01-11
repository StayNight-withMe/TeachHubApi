using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Category.input;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Service
{
    public interface ICategoryService
    {
    public Task<TResult<PagedResponseDTO<CategoryResponseDTO>>> SearchCategory(
    string searchText,
    PaginationDTO pagination,
    CancellationToken cancellationToken = default);
    }
}
