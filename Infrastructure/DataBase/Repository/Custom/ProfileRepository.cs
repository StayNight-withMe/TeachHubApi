using Application.Abstractions.Repository.Custom;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Profile.common;
using infrastructure.DataBase.Context;
using infrastructure.DataBase.Repository.Base;
using Microsoft.EntityFrameworkCore;


namespace infrastructure.DataBase.Repository.Custom
{
    public class ProfileRepository : BaseRepository<ProfileEntity>, IProfileRepository
    {
        public ProfileRepository(CourceDbContext courceDbContext) : base(courceDbContext)
        {
        }

        public async Task PartialUpdateAsync(ProfileEntity profile, ChangeProfileDTO dto)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            if (dto == null) return;

            if (dto.bio != null)
                profile.bio = dto.bio;

            if (dto.sociallinks != null)
            {
                profile.sociallinks.Clear();
                foreach (var link in dto.sociallinks)
                    profile.sociallinks[link.Key] = link.Value;
            }

            _db.Attach(profile);
            _db.Entry(profile).State = EntityState.Modified;
        }
    }
}
