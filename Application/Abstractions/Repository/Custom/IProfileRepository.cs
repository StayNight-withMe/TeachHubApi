using Application.Abstractions.Repository.Base;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Profile.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Repository.Custom
{
    public interface IProfileRepository : IBaseRepository<ProfileEntity>
    {
        Task PartialUpdateAsync(ProfileEntity profile, ChangeProfileDTO dto);
    }
}
