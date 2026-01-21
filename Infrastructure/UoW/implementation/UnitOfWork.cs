
using Application.Abstractions.UoW;
using infrastructure.DataBase.Context;

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
