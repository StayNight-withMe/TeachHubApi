
using AutoMapper;
using Core.Models.TargetDTO.Users.output;
using Core.Models.TargetDTO.Users.input;
using Core.Models.Entitiеs;


namespace Application.Mapping.AutoMapperProfiles
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

