using infrastructure.Utils.Mapping.MapperDTO;
using infrastructure.Entitiеs;
using AutoMapper;
using Core.Model.BaseModel.User;
using Core.Model.TargetDTO.Users.output;
using Core.Model.TargetDTO.Auth.input;
using Core.Model.BaseModel.Auth;
using Core.Model.TargetDTO.Users.input;


namespace infrastructure.Utils.Mapping.AutoMapperProfiles
{
    public class UsersMapperProfile : Profile
    {
        public UsersMapperProfile() 
        {
            CreateMap<RegistrationUserDto, UserEntities>(); 
            CreateMap<UserEntities, UserDto>();
            CreateMap<RegistrationUserDto, UserRoleEntities>();

            CreateMap<RoleEntities, RoleDto>();

        }

    }
        
    }

