namespace Core.Interfaces.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        public Task<T?> GetByIdAsync(CancellationToken ct = default, params object[] id);
        public Task Update(T value);
        public Task DeleteById(CancellationToken ct = default, params object[] id);
        public Task Create(T value);
        public Task PartialUpdateAsync(T entityToUpdate, object partialDTO);
        public IQueryable<T> GetAll();
        public IQueryable<T> GetAllWithoutTracking();

    }
}
