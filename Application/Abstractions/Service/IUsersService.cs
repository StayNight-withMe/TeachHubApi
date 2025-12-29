using Core.Common;
using Core.Model.BaseModel.Auth;
using Core.Model.BaseModel.User;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Auth.input;
using Core.Model.TargetDTO.Users.input;
using Core.Model.TargetDTO.Users.output;

namespace Application.Abstractions.Service
{
    public interface IUsersService
    {
        public Task<TResult<UserAuthDto>> RegistrationUser(
            RegistrationUserDto registrationUserDto, 
            PublicRole role, 
            string ip, 
            string userAgent,
            CancellationToken ct = default
            );
        public Task<TResult<checkEmailDTO>> CheckEmail(string email);
        public Task<TResult> SoftDelete(int id, CancellationToken ct = default);
        public Task<TResult> AgressiveRemoveUser(int id, CancellationToken ct = default);
    }
}
