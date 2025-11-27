using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Common.input;
using Core.Model.TargetDTO.Common.output;
using Core.Model.TargetDTO.Favorit.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Service
{
    public interface IFavoritService
    {
        Task<TResult<PagedResponseDTO<FavoritOutputDTO>>> GetFavorite(int userid, UserSortingRequest sort);
        Task<TResult> DeleteFavorit(int userid, int courseid);
        Task<TResult> CreateFavorite(int userid, int coureid);
    }
}
