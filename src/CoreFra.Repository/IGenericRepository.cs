﻿using CoreFra.Domain;
using System;
using System.Collections.Generic;
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


        void Update(TEntity entity);
        Task UpdateAsync(TEntity entity);

        void Delete(TEntity entity);
        void Delete(int id);
        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(int id);

        void SaveChanges();
        Task SaveChangesAsync();
    }
}