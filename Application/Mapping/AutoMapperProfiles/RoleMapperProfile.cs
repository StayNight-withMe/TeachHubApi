using AutoMapper;
using infrastructure.DataBase.Entitiеs;
using Application.Mapping.MapperDTO;

namespace Application.Mapping.AutoMapperProfiles
{
    public class RoleMapperProfile : Profile
    {

        public RoleMapperProfile()
        {

            CreateMap<UserRoleMappingSource, UserRoleEntity>()


             .ForMember(
                 dest => dest.userid,
                 opt => opt.MapFrom(src => src.User.id)
             )


             .ForMember(
                 dest => dest.roleid,
                 opt => opt.MapFrom(src => src.RoleId)
             );

        }

    }
}
