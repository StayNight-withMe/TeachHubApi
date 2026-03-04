using AutoMapper;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Lesson.input;
using Core.Models.TargetDTO.Lesson.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapping.AutoMapperProfiles
{
    public class LessonMapperProfile : Profile
    {
        public LessonMapperProfile()
        {

            CreateMap<createLessonDTO, LessonEntity>();

            CreateMap<LessonEntity, lessonOutputDTO>();


        }
    }
}
