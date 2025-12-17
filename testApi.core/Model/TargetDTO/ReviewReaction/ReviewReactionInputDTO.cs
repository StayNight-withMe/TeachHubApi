using Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.TargetDTO.ReviewReaction
{
    public class ReviewReactionInputDTO
    {
        public int reviewId { get; set; }
        public ReactionType reactiontype { get; set; }
    }
}
