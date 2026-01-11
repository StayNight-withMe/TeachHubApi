using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Auth.input;
using Core.Models.TargetDTO.Users.input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Service
{
    public interface IAuthService
    {
        public Task<TResult<UserAuthDto>> LoginUser(
            LoginUserDTO loginUserDTO, 
            string ip, 
            string userAgent,
            CancellationToken ct = default
            );
    }
}
