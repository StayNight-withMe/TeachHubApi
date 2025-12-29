using Core.Model.ReturnEntity;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Runtime.CompilerServices;
using AutoMapper;
using Core.Model.TargetDTO.Auth.input;
using Core.Model.TargetDTO.Users.input;
using Core.Common.EnumS;
using infrastructure.DataBase.Entitiеs;
using Application.Abstractions.Repository.Base;
using Application.Abstractions.Service;


namespace Application.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IJwtService _jwtService;

        private readonly IBaseRepository<UserEntity> _userRepository;

        private readonly IBaseRepository<RoleEntity> _roleRepository;

        private readonly IBaseRepository<UserRoleEntity> _userRolesRepository;
        
        private readonly ILogger<AuthService> _logger;

        private readonly IMapper _mapper;
        public AuthService(IJwtService jwtService, 
            IBaseRepository<UserEntity> userRepository,  
            IBaseRepository<RoleEntity> roleRepository,
            IBaseRepository<UserRoleEntity> userRoleRepository,
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
                return TResult<UserAuthDto>.FailedOperation(errorCode.IpNotFound);
            }
            var user = await _userRepository
                .GetAll()
                .Where(c => c.email == loginUserDTO.email && 
                c.isdelete == false)
                .FirstOrDefaultAsync(ct);


            if(user != null)
            {
                if(!PasswordHashService.VerifyPassword(loginUserDTO.password, user.password))
                {
                    return TResult<UserAuthDto>.FailedOperation(errorCode.PasswordDontMatch);
                }


                UserRoleEntity? userRole = await _userRolesRepository.GetByIdAsync(ct, user.id, (int)loginUserDTO.role);
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
