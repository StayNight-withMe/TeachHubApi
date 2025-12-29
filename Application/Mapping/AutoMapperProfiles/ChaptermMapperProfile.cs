using Application.Mapping.MapperDTO;
using AutoMapper;
using Core.Model.TargetDTO.Chapter.input;
using Core.Model.TargetDTO.Chapter.output;
using infrastructure.DataBase.Entitiеs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapping.AutoMapperProfiles
{
    public class ChaptermMapperProfile : Profile
    {
        public ChaptermMapperProfile() 
        {
            CreateMap<CreateChapterDTO, ChapterEntity>();
            CreateMap<ChapterMappingSource, ChapterEntity>()
               .ForMember(
                c => c.id,
                c => c.MapFrom(c => c.Chapter.id)
                )

                .ForMember(
                c => c.name,
                c => c.MapFrom(c => c.Chapter.name ?? null)
                )

                .ForMember(
                c => c.courseid,
                c => c.MapFrom(c => c.courseid)
                )
                
                .ForMember(
                c => c.order,
                c => c.MapFrom(c => c.Chapter.order)
                );

            CreateMap<ChapterEntity, ChapterOutDTO>();

        }
    }
}
