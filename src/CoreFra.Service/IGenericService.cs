using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreFra.Domain;

namespace CoreFra.Service
{
    public interface IGenericService<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Queryable();
        IEnumerable<TEntity> GetAll();
        PagedCollection<TEntity> GetPagedCollection(int pageNumber = 1, int pageSize = 10);
        TEntity FindById(int id);
        Task<TEntity> FindByIdAsync(int id);


        void Insert(TEntity entity);
        Task InsertAsync(TEntity entity);
        void BulkInsert(IEnumerable<TEntity> entities);
        Task BulkInsertAsync(IEnumerable<TEntity> entities);


        void Update(TEntity entity);
        Task UpdateAsync(TEntity entity);

        void Delete(int id);
        Task DeleteAsync(int id);

        void SaveChanges();
        Task SaveChangesAsync();
    }
}