using AutoMapper;
using AutoMapper;
using Core.Common;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Interfaces.UoW;
using Core.Model.BaseModel.Auth;
using Core.Model.BaseModel.User;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.Auth.input;
using Core.Model.TargetDTO.Users.input;
using Core.Model.TargetDTO.Users.output;
using infrastructure.Entitiеs;
using infrastructure.Utils.Mapping.MapperDTO;
using infrastructure.Utils.PasswodHashService;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Authentication;

namespace Applcation.Service.UserService
{
    public class UserService : IUsersService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IBaseRepository<UserEntities> _userRepository;

        private readonly IBaseRepository<RoleEntities> _roleRepository;

        private readonly IBaseRepository<UserRoleEntities> _userRoleRepository;

        private readonly ILogger<IUsersService> _logger;

        private readonly IMapper _mapper;

       

        public UserService(
            IBaseRepository<UserEntities> repository, 
            IBaseRepository<RoleEntities> RoleRepository,
            IBaseRepository<UserRoleEntities> userRoleRepository, 
            IUnitOfWork transaction, 
            IMapper mapper, 
            ILogger<IUsersService> logger
            ) 
        {
            _unitOfWork = transaction;
            _userRepository = repository;
            _roleRepository = RoleRepository;
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
            _logger = logger;
         
        }

     
        public async Task<TResult<UserAuthDto>> RegistrationUser(RegistrationUserDto  registrationUserDto, PublicRole role, string ip, string UA)
        {

           
            int Emailcount =  await _userRepository.GetAllWithoutTracking().CountAsync(c => c.email == registrationUserDto.email);
            if(Emailcount > 0)
            {
                TResult.FailedOperation(errorCode.UserAlreadyExists, "такой email уже существует");
            }

            registrationUserDto.password = PasswordHashService.PasswordHashing(registrationUserDto.password);
            UserEntities userEntities = _mapper.Map<UserEntities>(registrationUserDto);
            await _userRepository.Create(userEntities);

            var roleSource = new UserRoleMappingSource
            {
                User = userEntities,
                RoleId = (int)role,
            };
           
            await _userRoleRepository.Create(
                            _mapper.Map<UserRoleEntities>(roleSource));


            var authSource = new UserAuthMappingSource
            {
                user = userEntities,
                role = Enum.GetName(role),
                ip = ip,
                UserAgent = UA,
            };


            


            try
            {
                
                int count = await _unitOfWork.CommitAsync();
                var userAuth = _mapper.Map<UserAuthDto>(authSource);
                _logger.LogAffectedRows(count);
                return TResult<UserAuthDto>.CompletedOperation(userAuth);
            }
            catch(DbUpdateException ex) 
            {
                _logger.LogError(ex.Message);
                return TResult<UserAuthDto>.FailedOperation(errorCode.UnknownError, "ошибка регистрации");
            }      
        }


        private bool IsValidEmail(string email)
        {
            try { return new System.Net.Mail.MailAddress(email).Address == email; }  // Простая проверка
            catch { return false; }
        }

        public async Task<TResult<checkEmailDTO>> CheckEmail(string email)
        {
            if(!IsValidEmail(email))
            {
                return TResult<checkEmailDTO>.FailedOperation(errorCode.EmailInvalid);
            }
            bool isTaken = await _userRepository.GetAllWithoutTracking().AnyAsync(c => c.email == email);
            string message;
           
            if (isTaken)
            {
                message = "Этот email уже используется";
            }
            else
            {
                message = "Email доступен для регистрации";

            }
            return TResult<checkEmailDTO>.CompletedOperation(new checkEmailDTO { message = message, available = isTaken } );
            //return TResult.FailedOperation(errorCode.EmailAlreadyExists);
        }



        public async Task<TResult> AgressiveRemoveUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogRemoveUserNotFound(id);
                return TResult<UserDto>.FailedOperation(errorCode.UserNotFound, "ошибка удаления пользователь не найден");
            }
            await _userRepository.DeleteById(id);
            var count =  await _unitOfWork.CommitAsync();
            return TResult.CompletedOperation();
        }


        public async Task<TResult> SoftDelete(int id)
        {
            UserEntities? user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogRemoveUserNotFound(id);
                return TResult.FailedOperation(errorCode.UserNotFound, "ошибка удаления пользователь не найден");
            }
             
            user.isdelete = true;
            await _userRepository.Update(user);
            var count = await _unitOfWork.CommitAsync();
            _logger.LogAffectedRows(count);
            return TResult.CompletedOperation();
            
        }
    }
}
