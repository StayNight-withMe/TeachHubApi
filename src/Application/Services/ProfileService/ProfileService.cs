using Application.Abstractions.Repository.Custom;
using Application.Abstractions.Service;
using Application.Abstractions.UoW;
using Application.Abstractions.Utils;
using Core.Common.EnumS;
using Core.Common.Exeptions;
using Core.Models.Entitiеs;
using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Profile.common;
using Core.Models.TargetDTO.Profile.input;
using Core.Models.TargetDTO.Profile.output;
using Core.Specification.Profile;

namespace Application.Services.ProfileService
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileImageService _profileFileService;

        private readonly IProfileRepository _profileRepository;

        private readonly IUnitOfWork _unitOfWork;


        public ProfileService(IProfileImageService profileFileService, 
            IProfileRepository profileRepository,
            IUnitOfWork unitOfWork) 
        {
            _profileFileService = profileFileService;
            _profileRepository = profileRepository;
            _unitOfWork = unitOfWork;
        }



        public async Task<TResult<ChangeProfileDTO>> ChangeProfile(
            ChangeProfileDTO dto, 
            int userid, 
            CancellationToken ct = default)
        {
            try
            {
                var profileEntity = await _profileRepository.GetByIdAsync(ct, userid);
                if(profileEntity == null)
                {
                    return TResult<ChangeProfileDTO>.FailedOperation(errorCode.NotFound);
                }
                await _profileRepository.PartialUpdateAsync(profileEntity, dto);
                await _unitOfWork.CommitAsync(ct);
                return TResult<ChangeProfileDTO>.CompletedOperation(dto);
            }
            catch(DbUpdateException)
            {
                return TResult<ChangeProfileDTO>.FailedOperation(errorCode.DatabaseError);
            }
            catch(Exception ex)
            {
                return TResult<ChangeProfileDTO>.FailedOperation(errorCode.UnknownError);
            }

        }

        public async Task<TResult<string>> ChangeProfileIcon(
            Stream stream, 
            int userid,
            ProfileSetImageDTO dto, 
            string contentType,
            CancellationToken ct = default)
        {
            ProfileEntity? entity = await _profileRepository.GetByIdAsync(userid, ct);

            switch (dto.setstatus)
            {
                case SetImageStatus.Upload:
                    {
                        try
                        {
                            if (entity.avatarkey != null)
                            {
                                await _profileFileService.DeleteFileAsync(
                                    entity.avatarkey, 
                                    ct);
                            }
                            string key = await _profileFileService.UploadFileAsync(
                                stream, 
                                entity.userid, 
                                contentType, 
                                "icon", 
                                ct);

                            entity.avatarkey = key;

                            return TResult<string>.CompletedOperation(key);
                            
                        }
                        catch(DbUpdateException)
                        {
                            return TResult<string>.FailedOperation(errorCode.DatabaseError);
                        }
                        catch (Exception ex)
                        {
                            return TResult<string>.FailedOperation(errorCode.UnknownError);
                        }
  
                };
                case SetImageStatus.Remove:
                {
                        entity.avatarkey = null;

                        try
                        {
                            var count = await _unitOfWork.CommitAsync(ct);
                            if (count == 0)
                            {
                                return TResult<string>.FailedOperation(errorCode.NotFound);
                            }
                            return TResult<string>.CompletedOperation(null);
                        }
                        catch (Exception ex)
                        {
                            return TResult<string>.FailedOperation(errorCode.UnknownError);
                        }
                    };
                default:
                    return TResult<string>.FailedOperation(errorCode.InvalidDataFormat);

            }
  
            


            }
        

        public async Task<TResult<ProfileOutputDTO>> GetUserProfile(
            int userid, 
            CancellationToken ct = default)
        {
            var entity = await _profileRepository.FirstOrDefaultAsync(new UserProfileSpec(userid), ct);

            if (entity == null)
            {
                return TResult<ProfileOutputDTO>.FailedOperation(errorCode.NotFound);
            }

            return TResult<ProfileOutputDTO>.CompletedOperation(new ProfileOutputDTO()
            {
                bio = entity.bio,
                iconurl = _profileFileService.GetPresignedUrl(entity.avatarkey, 60),
                name = entity.user.name,
                sociallinks = entity.sociallinks,
                userid = entity.userid
            });
        }
    }
}
