using infrastructure.DataBase.Entitiеs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapping.AutoMapperProfiles
{
    public class ReviewReactionMapperProfile : AutoMapper.Profile
    {
        public ReviewReactionMapperProfile()
        {
            CreateMap<Core.Model.TargetDTO.ReviewReaction.ReviewReactionInputDTO, ReviewreactionEntity>();
        }
    }
}
