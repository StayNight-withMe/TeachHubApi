using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applcation.Abstractions.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        public Task<int> CommitAsync(CancellationToken ct = default);
    }
}
