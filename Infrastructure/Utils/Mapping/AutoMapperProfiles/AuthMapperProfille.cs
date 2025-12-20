using AutoMapper;
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
