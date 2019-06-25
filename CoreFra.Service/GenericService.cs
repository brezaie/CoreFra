using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreFra.Domain;
using CoreFra.Repository;

namespace CoreFra.Service
{
    public class GenericService<TEntity, TRepository> : IGenericService<TEntity> where TEntity : class
        where TRepository : IGenericRepository<TEntity>
    {
        protected TRepository Repository { get; set; }

        public GenericService(TRepository repository)
        {
            Repository = repository;
        }

        public IQueryable<TEntity> Queryable()
        {
            return Repository.Queryable();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Repository.GetAll();
        }

        public PagedCollection<TEntity> GetPagedCollection(int pageNumber = 1, int pageSize = 10)
        {
            return Repository.GetPagedCollection(pageNumber, pageSize);
        }

        public TEntity FindById(int id)
        {
            return Repository.FindById(id);
        }

        public Task<TEntity> FindByIdAsync(int id)
        {
            return Repository.FindByIdAsync(id);
        }

        public void Insert(TEntity entity)
        {
            Repository.Insert(entity);
        }

        public Task InsertAsync(TEntity entity)
        {
            return Repository.InsertAsync(entity);
        }

        public void BulkInsert(IEnumerable<TEntity> entities)
        {
            Repository.BulkInsert(entities);
        }

        public Task BulkInsertAsync(IEnumerable<TEntity> entities)
        {
            return Repository.BulkInsertAsync(entities);
        }

        public void Update(TEntity entity)
        {
            Repository.Update(entity);
        }

        public Task UpdateAsync(TEntity entity)
        {
            return Repository.UpdateAsync(entity);
        }

        public void Delete(int id)
        {
            Repository.Delete(id);
        }

        public Task DeleteAsync(int id)
        {
            return Repository.DeleteAsync(id);
        }

        public void SaveChanges()
        {
            Repository.SaveChanges();
        }

        public Task SaveChangesAsync()
        {
            return Repository.SaveChangesAsync();
        }
    }
}