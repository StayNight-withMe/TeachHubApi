using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;
using Core.Models.TargetDTO.Favorit.output;

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
