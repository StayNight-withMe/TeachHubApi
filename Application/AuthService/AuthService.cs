using Core.Model.ReturnEntity;
using Core.Interfaces.Service;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Runtime.CompilerServices;
using AutoMapper;
using infrastructure.Utils.Mapping.MapperDTO;
using Core.Common;
using Core.Interfaces.Repository;
using infrastructure.Entitiеs;
using Microsoft.AspNetCore.Http;
using infrastructure.Utils.HeadersService;
using Core.Model.TargetDTO.Auth.input;
using Core.Model.TargetDTO.Users.input;
using infrastructure.Utils.PasswodHashService;
using Microsoft.EntityFrameworkCore;


namespace Applcation.Service.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IJwtService _jwtService;

        private readonly IBaseRepository<UserEntities> _userRepository;

        private readonly IBaseRepository<RoleEntities> _roleRepository;

        private readonly IBaseRepository<UserRoleEntities> _userRolesRepository;
        
        private readonly ILogger<AuthService> _logger;

        private readonly IMapper _mapper;
        public AuthService(IJwtService jwtService, 
            IBaseRepository<UserEntities> userRepository,  
            IBaseRepository<RoleEntities> roleRepository,
            IBaseRepository<UserRoleEntities> userRoleRepository,
            ILogger<AuthService> logger,
            IMapper mapper
            )
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
            _logger = logger;
            _userRolesRepository = userRoleRepository;
            _mapper = mapper;
        }


        public async Task<TResult<UserAuthDto>> LoginUser(
            LoginUserDTO loginUserDTO, 
            string ip, 
            string userAgent,
            CancellationToken ct = default)  
        {
            
            if (ip == string.Empty)
            {
                return TResult<UserAuthDto>.FailedOperation(errorCode.IpNotFound, "ошибка аутентификации, ip неизвествен");
            }
            var user = await _userRepository
                .GetAll()
                .Where(c => c.email == loginUserDTO.email && c.isdelete == false)
                .FirstOrDefaultAsync(ct);
            if(user != null)
            {
                if(!PasswordHashService.VerifyPassword(loginUserDTO.password, user.password))
                {
                    return TResult<UserAuthDto>.FailedOperation(errorCode.PasswordDontMatch);
                }


                UserRoleEntities? userRole = await _userRolesRepository.GetByIdAsync(ct, user.id, (int)loginUserDTO.role);
                if (userRole != null)
                {
                   // Console.WriteLine($" IDROLE {userRole.roleid}, USERID {userRole.user.id}");
                    AllRole role = (AllRole)userRole.roleid;
                    var userAuthSource = new UserAuthMappingSource
                    {
                        role =  Enum.GetName(role),
                        user = user,
                        ip = ip,
                        UserAgent = userAgent,
                    };
                    var auth = _mapper.Map<UserAuthDto>(userAuthSource);
                 
                    return TResult<UserAuthDto>.CompletedOperation(auth);
                }

                
            }
            return TResult<UserAuthDto>.FailedOperation(errorCode.UserNotFound);
        }

    }
}
