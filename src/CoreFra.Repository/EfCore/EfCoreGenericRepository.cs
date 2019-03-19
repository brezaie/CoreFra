using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CoreFra.Repository.EfCore
{
    public class EfCoreGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;
        private readonly IQuery<TEntity> _query;

        #region Constructor

        public EfCoreGenericRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
            _query = new Query<TEntity>(this);
        }

        #endregion

        public virtual IQueryable<TEntity> Queryable()
        {
            return _dbSet;
        }

        public virtual IQueryable<TEntity> Queryable(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return _dbSet.ToList();
        }

        public virtual TEntity FindById(int id)
        {
            return _dbSet.Find(id);
        }

        public virtual Task<TEntity> FindByIdAsync(int id)
        {
            return _dbSet.FindAsync(id);
        }

        public virtual void Insert(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Added;
            _dbSet.Add(entity);
        }

        public virtual async Task InsertAsync(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Added;
            await _dbSet.AddAsync(entity);
        }

        public virtual void BulkInsert(IEnumerable<TEntity> entities, SqlTransaction transaction = null, int batchSize = 0, int bulkCopyTimeout = 30)
        {
            throw new NotImplementedException();
        }

        public virtual Task BulkInsertAsync(IEnumerable<TEntity> entities, SqlTransaction transaction = null, int batchSize = 0, int bulkCopyTimeout = 30)
        {
            throw new NotImplementedException();
        }
        
        public virtual void Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbSet.Attach(entity);
        }

        public virtual Task UpdateAsync(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbSet.Attach(entity);
            return Task.CompletedTask;
        }

        public virtual void Delete(int id)
        {
            var existingEntity = FindById(id);
            if (existingEntity != null)
            {
                _dbContext.Entry(existingEntity).State = EntityState.Deleted;
                _dbSet.Attach(existingEntity);
            }
        }
        
        public virtual async Task DeleteAsync(int id)
        {
            var existingEntity = await FindByIdAsync(id);
            if (existingEntity != null)
            {
                _dbContext.Entry(existingEntity).State = EntityState.Deleted;
                _dbSet.Attach(existingEntity);
            }
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}