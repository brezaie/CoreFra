using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CoreFra.Repository
{
    public class EfCoreGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;
        private readonly IQuery<TEntity> _query;

        public EfCoreGenericRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
            _query = new Query<TEntity>(this);
        }

        public IQueryable<TEntity> Queryable()
        {
            return _dbSet;
        }

        public IQueryable<TEntity> Queryable(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _dbSet.ToList();
        }

        public TEntity FindById(int id)
        {
            return _dbSet.Find(id);
        }

        public Task<TEntity> FindByIdAsync(int id)
        {
            return _dbSet.FindAsync(id);
        }

        public void Insert(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Added;
            _dbSet.Add(entity);
            _dbContext.SaveChanges();
        }

        public async Task InsertAsync(TEntity entity)
        {
            //if (_dbContext.Entry(entity).State != EntityState.Added)
            _dbContext.Entry(entity).State = EntityState.Added;
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public void Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbSet.Attach(entity);
            _dbContext.SaveChanges();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbSet.Attach(entity);
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Deleted;
            _dbSet.Attach(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var existingEntity = FindById(id);
            if (existingEntity != null)
                Delete(existingEntity);
        }

        public Task DeleteAsync(TEntity entity)
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