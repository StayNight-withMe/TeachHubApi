using AutoMapper;
using Core.Common.EnumS;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Interfaces.UoW;
using Core.Model.ReturnEntity;
using Core.Model.TargetDTO.ReviewReaction;
using infrastructure.DataBase.Entitiеs;
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
            var reaction = await 
                _reviewReactionRepository
                .GetAll()
                .Where(c => c.reviewid == reactionDTO.reviewId &&
                       c.userid == userId)
                .FirstOrDefaultAsync(ct);

            
            if (reaction == null)
            {
                if (reactionDTO.reactiontype != reaction_type.None)
                {
                    var entity = _mapper.Map<ReviewreactionEntity>(reactionDTO);
                    entity.userid = userId;
                    await _reviewReactionRepository.Create(entity);
                }
                else
                {
                    return TResult.CompletedOperation();
                }
            }
            else if (reaction.reactiontype == reactionDTO.reactiontype)
            {
                return TResult.CompletedOperation();
            }
            else if (reactionDTO.reactiontype == reaction_type.None)
            {
                await _reviewReactionRepository.DeleteById(ct, reaction.id);
            }
            else
            {
                reaction.reactiontype = reactionDTO.reactiontype;
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
