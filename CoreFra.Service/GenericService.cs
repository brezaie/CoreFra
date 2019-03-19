using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreFra.Repository;

namespace CoreFra.Service
{
    public class GenericService<TEntity, TRepository> : IGenericService<TEntity> where TEntity : class
        where TRepository : IGenericRepository<TEntity>
    {
        private TRepository Repository { get; set; }

        public IQueryable<TEntity> Queryable()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<TEntity> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public TEntity FindById(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<TEntity> FindByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public void Insert(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public Task InsertAsync(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public void BulkInsert(IEnumerable<TEntity> entities)
        {
            throw new System.NotImplementedException();
        }

        public Task BulkInsertAsync(IEnumerable<TEntity> entities)
        {
            throw new System.NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(TEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public void SaveChanges()
        {
            throw new System.NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}