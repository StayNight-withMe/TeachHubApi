using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.ReviewReaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Service
{
    public interface IReviewReactionService
    {
        Task<TResult> PutReaction(
            ReviewReactionInputDTO reactionDTO,
            int userId,
            CancellationToken ct = default);
    }
}
