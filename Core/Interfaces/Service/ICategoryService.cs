using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Category.input;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Service
{
    public interface ICategoryService
    {
    public Task<TResult<PagedResponseDTO<CategoryResponseDTO>>> SearchCategory(
    string searchText,
    PaginationDTO pagination,
    CancellationToken cancellationToken = default);
    }
}
