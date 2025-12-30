using Core.Model.ReturnEntity;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Application.Mapping.MapperDTO;
using Core.Model.TargetDTO.Auth.input;
using Core.Model.TargetDTO.Users.input;
using Core.Common.EnumS;
using infrastructure.DataBase.Entitiеs;
using Application.Abstractions.Repository.Base;
using Application.Abstractions.Service;
using Core.Specification.AuthSpec;
using Application.Utils.PasswodHashService;

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
            var pasword = await _userRepository.FirstOrDefaultAsync(new UserAuthSpec(loginUserDTO.email));


            if(pasword != null)
            {
                if(!PasswordHashService.VerifyPassword(loginUserDTO.password, pasword))
                {
                    return TResult<UserAuthDto>.FailedOperation(errorCode.PasswordDontMatch);
                }


                UserRoleEntity? userRole = await _userRolesRepository.GetByIdAsync(ct, pasword.id, (int)loginUserDTO.role);
                if (userRole != null)
                {
                    AllRole role = (AllRole)userRole.roleid;
                    var userAuthSource = new UserAuthMappingSource
                    {
                        role =  Enum.GetName(role),
                        user = pasword,
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
