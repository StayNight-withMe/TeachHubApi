using AutoMapper;
using Core.Model.TargetDTO.Lesson.input;
using Core.Model.TargetDTO.Lesson.output;
using infrastructure.DataBase.Entitiеs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Utils.Mapping.AutoMapperProfiles
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
