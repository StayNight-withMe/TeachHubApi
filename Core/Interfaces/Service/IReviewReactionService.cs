using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.ReviewReaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Service
{
    public interface IReviewReactionService
    {
        Task<TResult> PutReaction(
            ReviewReactionInputDTO reactionDTO,
            int userId,
            CancellationToken ct = default);
    }
}
