using AutoMapper;
using infrastructure.Utils.Mapping.MapperDTO;
using infrastructure.DataBase.Entitiеs;

namespace infrastructure.Utils.Mapping.AutoMapperProfiles
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
