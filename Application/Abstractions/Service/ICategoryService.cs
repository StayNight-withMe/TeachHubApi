using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Category.input;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;


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
