using Application.Abstractions.Repository.Base;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Profile.common;

namespace Application.Abstractions.Repository.Custom
{
    public interface IProfileRepository : IBaseRepository<ProfileEntity>
    {
        Task PartialUpdateAsync(ProfileEntity profile, ChangeProfileDTO dto);
    }
}
