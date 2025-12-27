
using Core.Interfaces.UoW;
using infrastructure.DataBase.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.UoW.implementation
{
    public class UnitOfWork : IUnitOfWork
    {
       private readonly CourceDbContext _db;
        
        public UnitOfWork(CourceDbContext courceDbContext) 
        {
        _db = courceDbContext;
        }

        public async Task<int> CommitAsync(CancellationToken ct = default)
        {
            return await _db.SaveChangesAsync(ct);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
