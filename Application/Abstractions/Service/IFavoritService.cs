using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Common.input;
using Core.Models.TargetDTO.Common.output;
using Core.Models.TargetDTO.Favorit.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Service
{
    public interface IFavoritService
    {
        Task<TResult<PagedResponseDTO<FavoritOutputDTO>>> GetFavorite(
            int userid, 
            SortingAndPaginationDTO sort, 
            CancellationToken ct = default);
        Task<TResult> DeleteFavorit(
            int userid, 
            int courseid,
            CancellationToken ct = default
            );
        Task<TResult> CreateFavorite(
            int userid, 
            int coureid,
            CancellationToken ct = default);
    }
}
