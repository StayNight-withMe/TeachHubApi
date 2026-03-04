using Core.Common.EnumS;
using Core.Common.Types.HashId;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.TargetDTO.ReviewReaction
{
    public class ReviewReactionInputDTO
    {
        public Hashid reviewId { get; set; }
        public reaction_type reactiontype { get; set; }
    }
}
