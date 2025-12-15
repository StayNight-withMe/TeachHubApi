using Core.Interfaces.Repository;
using infrastructure.Entitiеs;
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
                    IBaseRepository<UserEntities> repo,
                    CancellationToken ct = default);
        }
    
}
