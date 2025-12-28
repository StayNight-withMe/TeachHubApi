using AutoMapper;
using Core.Common.Types.HashId;
using Core.Model.TargetDTO.Users.input;
using infrastructure.Utils.Mapping.MapperDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace infrastructure.Utils.Mapping.AutoMapperProfiles
{
    public class AuthMapperProfile : Profile
    {

        public AuthMapperProfile() 
        {

            // Глобально для всего профиля:

            CreateMap<int?, Hashid?>().ConvertUsing(src => src.HasValue ? new Hashid(src.Value) : null);
            CreateMap<int, Hashid>().ConvertUsing(src => new Hashid(src));

            CreateMap<UserAuthMappingSource, UserAuthDto>()

         .ForMember(
         c => c.role,
         cc => cc.MapFrom(cc => cc.role)
         )

         .ForMember(
         c => c.id,
         cc => cc.MapFrom(cc => cc.user.id)

         )


         .ForMember(
         c => c.email,
         c => c.MapFrom(c => c.user.email)

         )

         .ForMember(
         c => c.name,
         c => c.MapFrom(c => c.user.name)
         )

          .ForMember(
         c => c.id,
         c => c.MapFrom(c => c.user.id)

         );



        }


    }
}
