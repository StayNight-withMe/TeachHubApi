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
        public int ReviewId { get; set; }
        public ReactionType reaction { get; set; }
    }
}
