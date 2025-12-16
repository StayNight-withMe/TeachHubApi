using AutoMapper;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Interfaces.UoW;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.ReviewReaction;
using infrastructure.Entitiеs;
using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applcation.Service.ReviewReactionService
{
    public class ReviewReactionService : IReviewReactionService
    {
        private readonly IBaseRepository<ReviewreactionEntities> _reviewReactionRepository;

        private readonly ILogger<ReviewReactionService> _logger;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper; 

        public ReviewReactionService(
            IBaseRepository<ReviewreactionEntities> reviewReactionRepository,
            ILogger<ReviewReactionService> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _reviewReactionRepository = reviewReactionRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            
        }
        public async Task<TResult> PutReaction(ReviewReactionInputDTO reactionDTO, int userId, CancellationToken ct = default)
        {
            var reaction = await _reviewReactionRepository
                .GetAll()
                .Where(c => c.reviewid == reactionDTO.ReviewId && 
                       c.userid == userId)
                .FirstOrDefaultAsync(ct);


            if(reaction == null)
            {
                if(reactionDTO.reaction != Core.Common.ReactionType.None)
                {
                    await _reviewReactionRepository.Create(_mapper.Map<ReviewreactionEntities>(reactionDTO));
                }
                else
                {
                    return TResult.CompletedOperation();
                }
            }
            else if (reaction.reactiontype == reactionDTO.reaction)
            {
                return TResult.CompletedOperation();
            }
            else if (reactionDTO.reaction == Core.Common.ReactionType.None)
            {
                await _reviewReactionRepository.DeleteById(ct, reaction.id);
            }
            else
            {
                reaction.reactiontype = reactionDTO.reaction;
            }
           

            try
            {
                await _unitOfWork.CommitAsync(ct);
                return TResult.CompletedOperation();
            }
            catch(DbUpdateException ex)
            {
                _logger.LogDBError(ex);
                return TResult.FailedOperation(errorCode.DatabaseError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return TResult.FailedOperation(errorCode.UnknownError);
            }

        }
    }
}
