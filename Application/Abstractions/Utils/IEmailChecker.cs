using Application.Abstractions.Repository.Base;
using Core.Models.Entitiеs;

namespace Application.Abstractions.Utils
{
 
        public interface IEmailChecker 
        {
            public bool EmailCheck(string email);
            public void AddEmail(string email);
            public Task RebuildFilter(
                    IBaseRepository<UserEntity> repo,
                    CancellationToken ct = default);
        }
    
}
