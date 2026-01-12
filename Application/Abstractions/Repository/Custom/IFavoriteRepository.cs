using Application.Abstractions.Repository.Base;
using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;
using Core.Models.TargetDTO.Favorit.output;
using Core.Models.Entitiеs;
namespace Application.Abstractions.Repository.Custom
{
    public interface IFavoriteRepository : IBaseRepository<FavoritEntity>
    {
        public  Task<TResult<PagedResponseDTO<FavoritOutputDTO>>> GetFavorite(
         int userid,
         SortingAndPaginationDTO sort,
         CancellationToken ct = default);
        }
}
