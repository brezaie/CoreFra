using CoreFra.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CoreFra.Repository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Queryable();
        IEnumerable<TEntity> GetAll();
        TEntity FindById(int id);
        Task<TEntity> FindByIdAsync(int id);


        void Insert(TEntity entity);
        Task InsertAsync(TEntity entity);
        void BulkInsert(IEnumerable<TEntity> entities, SqlTransaction transaction = null, int batchSize = 0, int bulkCopyTimeout = 30);
        Task BulkInsertAsync(IEnumerable<TEntity> entities, SqlTransaction transaction = null, int batchSize = 0, int bulkCopyTimeout = 30);


        void Update(TEntity entity);
        Task UpdateAsync(TEntity entity);

        void Delete(int id);
        Task DeleteAsync(int id);

        void SaveChanges();
        Task SaveChangesAsync();
    }
}