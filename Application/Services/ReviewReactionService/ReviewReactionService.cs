using Application.Abstractions.Repository.Base;
using Application.Abstractions.Service;
using Application.Abstractions.UoW;
using AutoMapper;
using Core.Common.EnumS;
using Core.Common.Exeptions; 
using Core.Models.Entitiеs;
using Core.Models.ReturnEntity;
using Core.Models.TargetDTO.ReviewReaction;
using Core.Specification.ReviewReactionSpec;
using Logger;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.ReviewReactionService
{
    public class ReviewReactionService : IReviewReactionService
    {
        private readonly IBaseRepository<ReviewreactionEntity> _reviewReactionRepository;

        private readonly ILogger<ReviewReactionService> _logger;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        public ReviewReactionService(
            IBaseRepository<ReviewreactionEntity> reviewReactionRepository,
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
        public async Task<TResult> PutReaction(
        ReviewReactionInputDTO reactionDTO,
        int userId,
        CancellationToken ct = default)
        {
  
            var reaction = await _reviewReactionRepository
                .FirstOrDefaultAsync(new ReviewReactionSpec(reactionDTO.reviewId, userId), ct);

            if (reaction == null)
            {
    
                if (reactionDTO.reactiontype != reaction_type.None)
                {
                    var entity = _mapper.Map<ReviewreactionEntity>(reactionDTO);
                    entity.userid = userId;

                    await _reviewReactionRepository.AddAsync(entity, ct);
                }
                else
                {
                 
                    return TResult.CompletedOperation();
                }
            }
            else
            {
              
                if (reaction.reactiontype == reactionDTO.reactiontype)
                {
          
                    return TResult.CompletedOperation();
                }

                if (reactionDTO.reactiontype == reaction_type.None)
                {
             
                    await _reviewReactionRepository.DeleteAsync(reaction, ct);
                }
                else
                {
                    reaction.reactiontype = reactionDTO.reactiontype;
                }
            }

            try
            {
                await _unitOfWork.CommitAsync(ct);
                return TResult.CompletedOperation();
            }
            catch (DbUpdateException ex)
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
