using Application.Abstractions.Repository.Base;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Core.Models.TargetDTO.Common.input;
using infrastructure.DataBase.Context;
using infrastructure.DataBase.Extensions;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.DataBase.Repository.Base
{
    public class BaseRepository<T> : RepositoryBase<T>, IBaseRepository<T> where T : class
    {
        protected readonly CourceDbContext _db;

        protected readonly DbSet<T> _dbSet;
        public BaseRepository(CourceDbContext courceDbContext) : base(courceDbContext)
        {
            
            _db = courceDbContext;
            _dbSet = _db.Set<T>();
        }

        /// <summary>
        /// создание сущности, для применения изменений нужен коммит изменений
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual async Task Create(T value)
        {
            if(value == null)
            {
                throw new ArgumentException("ошибка создания записи в базе данных : объект равен нулю");
            }
                _dbSet.Add(value);
                await Task.CompletedTask;
            
        }
        /// <summary>
        /// обновление сущности целиком, для применения изменений нужен коммит изменений
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual async Task Update(T value)
        {
            _dbSet.Update(value);
            await Task.CompletedTask;
        }

        /// <summary>
        /// удаление по id, поддерживается использование составных первичных ключей
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async virtual Task DeleteById(CancellationToken ct = default, params object[] id)
        {
            var value = await _dbSet.FindAsync(id, ct);
            if(value != null)
            {
                _dbSet.Remove(value);
                return;
            }
            throw new Core.Common.Exeptions.DbUpdateException("Not Found", "23503");
            
        }

        /// <summary>
        /// получение целого set используя IQueryable, сущности по умолчанию остаются прикреплены к EF
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }


        /// <summary>
        /// получения целого set используя IQueryable, сущности по умолчанию НЕ остаются прикреплены к EF 
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetAllWithoutTracking()
        {
            return _dbSet.AsQueryable().AsNoTracking();
        }


        protected IQueryable<T> GetWithPaginationAndSorting(IQueryable<T> qwery, SortingAndPaginationDTO dto)
        {
            return qwery.GetWithPaginationAndSorting(dto);
        }

        protected IQueryable<T> GetWithPagination(IQueryable<T> qwery, PaginationDTO dto)
        {
            return qwery.GetWithPagination(dto);
        }

        protected IQueryable<T> GetWithSorting(IQueryable<T> qwery, SortingDTO dto)
        {
            return qwery.GetWithSorting(dto);
        }

       


        /// <summary>
        /// получение объекта(записи) из сущности по id, присутствует поддержка составных первичных ключей
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T?> GetByIdAsync(CancellationToken ct = default, params object[] id)
        {
            return await _dbSet.FindAsync(id, ct);
        }

        /// <summary>
        /// частичное обновление, можно передавать не прикрепленные сущности, с primary key
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <param name="partialDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task PartialUpdateAsync(T entityToUpdate, object partialDto) 
        {
            if (entityToUpdate == null) throw new ArgumentNullException(nameof(entityToUpdate));
            if (partialDto == null) return;

            var entry = _db.Entry(entityToUpdate);

            if (entry.State == EntityState.Detached)
                _db.Set<T>().Attach(entityToUpdate);

            var dtoProps = partialDto.GetType().GetProperties();

            foreach (var prop in dtoProps)
            {
                if (prop.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                    continue;


                var value = prop.GetValue(partialDto);
                if (value == null)
                    continue;

        
                if (value != null || prop.PropertyType == typeof(string))
                {
                    var entityProperty = entry.Property(prop.Name);
                    entityProperty.CurrentValue = value;
                    if (entityProperty.Metadata.PropertyInfo != null) 
                    {
                        entityProperty.CurrentValue = value ?? ""; 
                        entityProperty.IsModified = true;
                    }
                }
            }
            

        }

    }
}
