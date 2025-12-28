using AutoMapper;
using Core.Model.TargetDTO.Courses.output;
using infrastructure.DataBase.Entitiеs;
using infrastructure.Utils.Mapping.MapperDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Utils.Mapping.AutoMapperProfiles
{
    public class CoursesMapperProfile : Profile
    {

        public CoursesMapperProfile()
        {

            CreateMap<CourcesMappingSource, CourseEntity>()

                .ForMember(
                c => c.name,
                c => c.MapFrom(c => c.CourceDTO.name)
                )


                .ForMember(
                c => c.description,
                c => c.MapFrom(c => c.CourceDTO.description)
                )

                .ForMember(
                c => c.creatorid,
                c => c.MapFrom(c => c.id)
                )

            .ForMember(c => c.id,
            opt => opt.Ignore());


            CreateMap<CourseEntity, CourseOutputDTO>();

        }


        


    }
}
