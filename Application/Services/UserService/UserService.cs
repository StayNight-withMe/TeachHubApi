using Application.Abstractions.Repository.Base;
using Application.Abstractions.Service;
using Application.Abstractions.UoW;
using AutoMapper;
using AutoMapper;
using Core.Common.EnumS;
using Core.Model.BaseModel.Auth;
using Core.Model.BaseModel.User;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Auth.input;
using Core.Model.TargetDTO.Users.input;
using Core.Model.TargetDTO.Users.output;
using infrastructure.DataBase.Entitiеs;
using infrastructure.Utils.BloomFilter.interfaces;
using infrastructure.Utils.Mapping.MapperDTO;
using infrastructure.Utils.PasswodHashService;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Authentication;


namespace Application.Services.UserService
{
    public class UserService : IUsersService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IBaseRepository<UserEntity> _userRepository;

        //private readonly IBaseRepository<RoleEntity> _roleRepository;

        private readonly IBaseRepository<UserRoleEntity> _userRoleRepository;

        private readonly IBaseRepository<ProfileEntity> _profileRepository;

        private readonly ILogger<IUsersService> _logger;

        private readonly IMapper _mapper;

        private readonly IEmailChecker _emailChecker;



        public UserService(
            IBaseRepository<UserEntity> repository, 
            //IBaseRepository<RoleEntity> RoleRepository,
            IBaseRepository<UserRoleEntity> userRoleRepository,
            IBaseRepository<ProfileEntity> profileRepository,
            IEmailChecker emailChecker,
            IUnitOfWork transaction, 
            IMapper mapper, 
            ILogger<IUsersService> logger
            ) 
        {
            _unitOfWork = transaction;
            _userRepository = repository;
            //_roleRepository = RoleRepository;
            _userRoleRepository = userRoleRepository;
            _profileRepository = profileRepository;
            _emailChecker = emailChecker;
            _mapper = mapper;
            _logger = logger;
         
        }

     
        public async Task<TResult<UserAuthDto>> RegistrationUser(
            RegistrationUserDto  registrationUserDto, 
            PublicRole role, 
            string ip, 
            string UA,
            CancellationToken ct = default
            )
        {

           
            int Emailcount =  
                await _userRepository
                .GetAllWithoutTracking()
                .CountAsync(c => c.email == registrationUserDto.email, ct);


            if(Emailcount > 0)
            {
                TResult.FailedOperation(errorCode.UserAlreadyExists);
            }

            registrationUserDto.password = PasswordHashService.PasswordHashing(registrationUserDto.password);
            UserEntity userEntities = _mapper.Map<UserEntity>(registrationUserDto);

            await _userRepository.Create(userEntities);
            await _profileRepository.Create(new ProfileEntity { user = userEntities, sociallinks = new Dictionary<string, string>() });


            var roleSource = new UserRoleMappingSource
            {
                User = userEntities,
                RoleId = (int)role,
            };
           
            await _userRoleRepository.Create(
                            _mapper.Map<UserRoleEntity>(roleSource));


            var authSource = new UserAuthMappingSource
            {
                user = userEntities,
                role = Enum.GetName(role),
                ip = ip,
                UserAgent = UA,
            };


            try
            {
                int count = await _unitOfWork.CommitAsync(ct);
                var userAuth = _mapper.Map<UserAuthDto>(authSource);
                _logger.LogAffectedRows(count);
                return TResult<UserAuthDto>.CompletedOperation(userAuth);
            }
            catch(DbUpdateException ex) 
            {
                _logger.LogError(ex.Message);
                return TResult<UserAuthDto>.FailedOperation(errorCode.UnknownError);
            }
            catch(Exception ex)
            {
                _logger.LogCriticalError(ex);
                return TResult<UserAuthDto>.FailedOperation(errorCode.UnknownError);
            }
        }


        private bool IsValidEmail(string email)
        {
            try { return new System.Net.Mail.MailAddress(email).Address == email; } 
            catch { return false; }
        }

        public async Task<TResult<checkEmailDTO>> CheckEmail(string email)
        {
            if(!IsValidEmail(email))
            {
                return TResult<checkEmailDTO>.FailedOperation(errorCode.EmailInvalid);
            }
            bool isTaken = _emailChecker.EmailCheck(email);

            return TResult<checkEmailDTO>.CompletedOperation(new checkEmailDTO { reasonCode = "TAKEN", available = isTaken } );
            //return TResult.FailedOperation(errorCode.EmailAlreadyExists);
        }



        public async Task<TResult> AgressiveRemoveUser(
            int id,
            CancellationToken ct = default
            )
        {
            var user = await _userRepository.GetByIdAsync(ct, id);
            if (user == null)
            {
                _logger.LogRemoveUserNotFound(id);
                return TResult<UserDto>.FailedOperation(errorCode.UserNotFound);
            }
            await _userRepository.DeleteById(ct, id);
            var count =  await _unitOfWork.CommitAsync();
            return TResult.CompletedOperation();
        }


        public async Task<TResult> SoftDelete(
            int id,
            CancellationToken ct = default)
        {
            UserEntity? user = await _userRepository.GetByIdAsync(ct, id);
            if (user == null)
            {
                _logger.LogRemoveUserNotFound(id);
                return TResult.FailedOperation(errorCode.UserNotFound);
            }
             
            user.isdelete = true;
            await _userRepository.Update(user);
            var count = await _unitOfWork.CommitAsync();
            _logger.LogAffectedRows(count);
            return TResult.CompletedOperation();
            
        }
    }
}
