using infrastructure.Utils.Mapping.MapperDTO;
using AutoMapper;
using Core.Model.BaseModel.User;
using Core.Model.TargetDTO.Users.output;
using Core.Model.TargetDTO.Auth.input;
using Core.Model.BaseModel.Auth;
using Core.Model.TargetDTO.Users.input;
using infrastructure.DataBase.Entitiеs;
using infrastructure.Utils.HashIdConverter;


namespace infrastructure.Utils.Mapping.AutoMapperProfiles
{
    public class UsersMapperProfile : Profile
    {
        public UsersMapperProfile() 
        {
            CreateMap<RegistrationUserDto, UserEntity>();

            //CreateMap<UserEntity, UserDto>().
            //    ForMember(
            //    c => c.id,
            //    c => c.MapFrom(c =>  (Hashid)c.id)
            //    )
            //;
            CreateMap<RegistrationUserDto, UserRoleEntity>();

            CreateMap<RoleEntity, RoleDto>();

        }

    }
        
    }

