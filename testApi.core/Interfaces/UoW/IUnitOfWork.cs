using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        public Task<int> CommitAsync(CancellationToken ct = default);
    }
}
