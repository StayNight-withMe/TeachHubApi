using AutoMapper;
using Core.Models.Entitiеs;
using Core.Models.TargetDTO.Review.input;
using Core.Models.TargetDTO.Review.output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapping.AutoMapperProfiles
{
    public class ReviewMapperProfile : Profile
    {
        public ReviewMapperProfile()
        {

            CreateMap<ReviewChangedDTO, ReviewEntities>()
                .ForMember(
                c => c.courseid,
                c => c.MapFrom(c => c.reviewid)
                )
                .ForMember(
                c => c.content,
                c => c.MapFrom(c => c.content)
                )
                .ForMember(
                c => c.review,
                c => c.MapFrom(c => c.review));


            CreateMap<ReviewEntities, ReviewOutputDTO>();

            CreateMap<ReviewICreateDTO, ReviewEntities>()
                .ForMember(
                c => c.courseid,
                c => c.MapFrom(c => c.courseid)
                )
                .ForMember(
                c => c.content,
                c => c.MapFrom(c => c.content)
                )
                .ForMember(
                c => c.review,
                c => c.MapFrom(c => c.review));

        }
    }
}
