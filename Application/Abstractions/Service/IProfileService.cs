using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Profile.common;
using Core.Models.TargetDTO.Profile.input;
using Core.Models.TargetDTO.Profile.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Service
{
    public interface IProfileService
    {
        Task<TResult<ChangeProfileDTO>> ChangeProfile(
            ChangeProfileDTO dto, 
            int userid, 
            CancellationToken ct = default);

        Task<TResult<string>> ChangeProfileIcon(
            Stream stream, 
            int userid,
            ProfileSetImageDTO dto,
            CancellationToken ct = default);

        Task<TResult<ProfileOutputDTO>> GetUserProfile(
            int userid,
            CancellationToken ct = default
            );
    }
}
