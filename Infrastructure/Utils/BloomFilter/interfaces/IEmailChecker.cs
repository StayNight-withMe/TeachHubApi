using Applcation.Abstractions.Repository.Base;
using infrastructure.DataBase.Entitiеs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Utils.BloomFilter.interfaces
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
