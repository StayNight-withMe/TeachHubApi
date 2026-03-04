using Microsoft.Extensions.Logging;
using AutoMapper;
using Application.Mapping.MapperDTO;
using Core.Common.EnumS;
using Application.Abstractions.Repository.Base;
using Application.Abstractions.Service;
using Core.Specification.AuthSpec;
using Application.Abstractions.Utils;
using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.Auth.input;
using Core.Models.TargetDTO.Users.input;
using Core.Models.Entitiеs;


namespace Application.Services.AuthService
{
    public class AuthService : IAuthService
    {
        //private readonly IJwtService _jwtService;

        private readonly IBaseRepository<UserEntity> _userRepository;

        //private readonly IBaseRepository<RoleEntity> _roleRepository;

        private readonly IBaseRepository<UserRoleEntity> _userRolesRepository;

        private readonly IPasswordHashService _passwordHashService;

        private readonly ILogger<AuthService> _logger;

        private readonly IMapper _mapper;
        public AuthService(IJwtService jwtService, 
            IBaseRepository<UserEntity> userRepository,  
            //IBaseRepository<RoleEntity> roleRepository,
            IBaseRepository<UserRoleEntity> userRoleRepository,
            IPasswordHashService passwordHashService,
            ILogger<AuthService> logger,
            IMapper mapper
            )
        {
            //_jwtService = jwtService;
            _userRepository = userRepository;
            _logger = logger;
            _userRolesRepository = userRoleRepository;
            _passwordHashService = passwordHashService;
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
            var user = await _userRepository.FirstOrDefaultAsync(new UserAuthSpec(loginUserDTO.email));


            if(user != null)
            {
                if(!_passwordHashService.VerifyPassword(loginUserDTO.password, user.password))
                {
                    return TResult<UserAuthDto>.FailedOperation(errorCode.PasswordDontMatch);
                }


                UserRoleEntity? userRole = await _userRolesRepository.GetByIdAsync(ct, user.id, (int)loginUserDTO.role);
                if (userRole != null)
                {
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
