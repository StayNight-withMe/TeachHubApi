

using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.Favorit.output;

namespace Application.Abstractions.Repository.Custom
{
    public interface IFavoriteRepository
    {
        public async Task<TResult<PagedResponseDTO<FavoritOutputDTO>>> GetFavorite(
     int userid,
     SortingAndPaginationDTO sort,
     CancellationToken ct = default);
    }
}
