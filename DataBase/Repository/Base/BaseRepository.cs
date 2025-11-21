using Core.Interfaces.Repository;
using infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace infrastructure.Repository.Base
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
       protected readonly CourceDbContext _db;

        protected readonly DbSet<T> _dbSet;
        public BaseRepository(CourceDbContext courceDbContext) 
        {
            _db = courceDbContext;
            _dbSet = _db.Set<T>();
        }

        public virtual async Task Create(T value)
        {
            if(value == null)
            {
                throw new ArgumentException("ошибка создания записи в базе данных : объект равен нулю");
            }
                _dbSet.Add(value);
                await Task.CompletedTask;
            
           
        }

        public virtual async Task Update(T value)
        {
            _dbSet.Update(value);
            await Task.CompletedTask;
        }

        public async virtual Task DeleteById(int id)
        {
            var value = await _dbSet.FindAsync(id);
            if(value != null)
            {
                _dbSet.Remove(value);
            }
        }


        public  IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }


        public IQueryable<T> GetAllWithoutTracking()
        {
            return _dbSet.AsQueryable().AsNoTracking();
        }

        public virtual async Task<T?> GetById(params object[] id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task PartialUpdateAsync(T entityToUpdate, object partialDto) 
        {
            if (entityToUpdate == null) throw new ArgumentNullException(nameof(entityToUpdate));
            if (partialDto == null) return;

            var entry = _db.Entry(entityToUpdate);

            // Если сущность не отслеживается — прикрепляем
            if (entry.State == EntityState.Detached)
                _db.Set<T>().Attach(entityToUpdate);

            var dtoProps = partialDto.GetType().GetProperties();

            foreach (var prop in dtoProps)
            {
                if (prop.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                    continue;

                // Получаем значение из DTO
                var value = prop.GetValue(partialDto);
                if (value == null)
                    continue;

                // Специально для строкам разрешаем "" (пустую строку)
                if (value != null || prop.PropertyType == typeof(string))
                {
                    var entityProperty = entry.Property(prop.Name);
                         Console.WriteLine($"Setting EF property '{prop.Name}' to {value ?? "NULL"}");
                    entityProperty.CurrentValue = value;
                    if (entityProperty.Metadata.PropertyInfo != null) // только реальные свойства
                    {
                        entityProperty.CurrentValue = value ?? ""; // или value, если не хочешь затирать null'ами
                        entityProperty.IsModified = true;
                    }
                }
            }

            
        }

    }
}
