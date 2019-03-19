using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace CoreFra.Repository.Dapper
{
    public class DapperGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        public IDbConnection Connection { get; }

        #region PrivateProperties

        private readonly string _tableName;

        #endregion

        #region Constructor

        public DapperGenericRepository(IDapperConnectionFactory dapperConnectionFactory)
        {
            Connection = dapperConnectionFactory.Connection;
            _tableName = typeof(TEntity).Name;
        }

        #endregion

        public IQueryable<TEntity> Queryable()
        {
            var query = $"SELECT * FROM {_tableName}";

            if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                Connection.Open();

            return Connection.Query<TEntity>(query).AsQueryable();
        }

        public IEnumerable<TEntity> GetAll()
        {
            var query = $"SELECT * FROM {_tableName}";

            if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                Connection.Open();

            return Connection.Query<TEntity>(query).ToList();
        }

        public TEntity FindById(int id)
        {
            var query = $"SELECT * FROM {_tableName} WHERE Id = @Id";

            if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                Connection.Open();

            var res = Connection.QueryFirstOrDefault<TEntity>(query, new { Id = id });
            return res;
        }

        public async Task<TEntity> FindByIdAsync(int id)
        {
            var query = $"SELECT * FROM {_tableName} WHERE Id = @Id";

            if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                Connection.Open();

            var res = await Connection.QueryFirstOrDefaultAsync<TEntity>(query, new { Id = id });
            return res;
        }

        public void Insert(TEntity entity)
        {
            if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                Connection.Open();

            var propertyContainer = DapperHelper.ParseProperties(entity);
            var query =
                $"INSERT INTO [{_tableName}] ({string.Join(", ", propertyContainer.ValueNames)}) " +
                $"VALUES(@{string.Join(", @", propertyContainer.ValueNames.Select(x => x.Replace("[", "").Replace("]", "")))})";

            Connection.Execute(query, propertyContainer.AllPairs, commandType: CommandType.Text);
        }

        public async Task InsertAsync(TEntity entity)
        {
            if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                Connection.Open();

            var propertyContainer = DapperHelper.ParseProperties(entity);
            var query =
                $"INSERT INTO [{_tableName}] ({string.Join(", ", propertyContainer.ValueNames)}) " +
                $"VALUES(@{string.Join(", @", propertyContainer.ValueNames.Select(x => x.Replace("[", "").Replace("]", "")))})";

            await Connection.ExecuteAsync(query, propertyContainer.AllPairs, commandType: CommandType.Text);
        }

        public void BulkInsert(IEnumerable<TEntity> entities, SqlTransaction transaction = null,
            int batchSize = 0, int bulkCopyTimeout = 30)
        {
            try
            {
                if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                    Connection.Open();

                var type = typeof(TEntity);
                var tableName = DapperHelper.GetTableName(type);
                var allProperties = DapperHelper.TypePropertiesCache(type);
                var keyProperties = DapperHelper.KeyPropertiesCache(type);
                //var computedProperties = ComputedPropertiesCache(type);
                var allPropertiesComputed = allProperties;
                var allPropertiesComputedString = DapperHelper.GetColumnsStringSqlServer(allPropertiesComputed);
                var tempToBeInserted = $"#{tableName}_TempInsert".Replace(".", string.Empty);

                Connection.Execute(
                    $@"IF OBJECT_ID('tempdb..#{tempToBeInserted}') IS NOT NULL drop table #{tempToBeInserted} SELECT TOP 0 {allPropertiesComputedString} INTO {tempToBeInserted} FROM {tableName} target WITH(NOLOCK);",
                    null, transaction);

                using (
                    var bulkCopy = new SqlBulkCopy((SqlConnection) Connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.BulkCopyTimeout = bulkCopyTimeout;
                    bulkCopy.BatchSize = batchSize;
                    bulkCopy.DestinationTableName = tempToBeInserted;
                    bulkCopy.WriteToServer(
                        DapperHelper.ToDataTable(entities, tableName, allPropertiesComputed).CreateDataReader());
                }

                Connection.Execute($@"
                INSERT INTO {tableName}({allPropertiesComputedString}) 
                SELECT {allPropertiesComputedString} FROM {tempToBeInserted}

                DROP TABLE {tempToBeInserted};", null, transaction);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task BulkInsertAsync(IEnumerable<TEntity> entities, SqlTransaction transaction = null,
            int batchSize = 0, int bulkCopyTimeout = 30)
        {
            try
            {
                if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                    Connection.Open();

                var type = typeof(TEntity);
                var tableName = DapperHelper.GetTableName(type);
                var allProperties = DapperHelper.TypePropertiesCache(type);
                var keyProperties = DapperHelper.KeyPropertiesCache(type);
                //var computedProperties = ComputedPropertiesCache(type);
                var allPropertiesComputed = allProperties;
                var allPropertiesComputedString = DapperHelper.GetColumnsStringSqlServer(allPropertiesComputed);
                var tempToBeInserted = $"#{tableName}_TempInsert".Replace(".", string.Empty);

                await Connection.ExecuteAsync(
                    $@"IF OBJECT_ID('tempdb..#{tempToBeInserted}') IS NOT NULL drop table #{tempToBeInserted} SELECT TOP 0 {allPropertiesComputedString} INTO {tempToBeInserted} FROM {tableName} target WITH(NOLOCK);",
                    null, transaction);

                using (var bulkCopy = new SqlBulkCopy((SqlConnection) Connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.BulkCopyTimeout = bulkCopyTimeout;
                    bulkCopy.BatchSize = batchSize;
                    bulkCopy.DestinationTableName = tempToBeInserted;
                    await bulkCopy.WriteToServerAsync(
                        DapperHelper.ToDataTable(entities, tableName, allPropertiesComputed).CreateDataReader());
                }

                await Connection.ExecuteAsync($@"
                INSERT INTO {tableName}({allPropertiesComputedString}) 
                SELECT {allPropertiesComputedString} FROM {tempToBeInserted}

                DROP TABLE {tempToBeInserted};", null, transaction);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public void Update(TEntity entity)
        {
            if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                Connection.Open();

            var propertyContainer = DapperHelper.ParseProperties(entity);
            var sqlIdPairs = DapperHelper.GetSqlPairs(propertyContainer.IdNames);
            var sqlValuePairs = DapperHelper.GetSqlPairs(propertyContainer.ValueNames);

            var query = $"UPDATE [{_tableName}] SET {sqlValuePairs} WHERE {sqlIdPairs}";
            Connection.Execute(query, propertyContainer.AllPairs, commandType: CommandType.Text);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                Connection.Open();

            var propertyContainer = DapperHelper.ParseProperties(entity);
            var sqlIdPairs = DapperHelper.GetSqlPairs(propertyContainer.IdNames);
            var sqlValuePairs = DapperHelper.GetSqlPairs(propertyContainer.ValueNames);

            var query = $"UPDATE [{_tableName}] SET {sqlValuePairs} WHERE {sqlIdPairs}";
            await Connection.ExecuteAsync(query, propertyContainer.AllPairs, commandType: CommandType.Text);
        }

        public void Delete(int id)
        {
            var type = typeof(TEntity);

            if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                Connection.Open();

            var propertyContainer = DapperHelper.ParseProperties(type);
            var sqlIdPairs = DapperHelper.GetSqlPairs(propertyContainer.IdNames);

            var query = $"DELETE FROM [{_tableName}] WHERE {sqlIdPairs}";
            Connection.Execute(query, propertyContainer.IdPairs, commandType: CommandType.Text);
        }

        public async Task DeleteAsync(int id)
        {
            var type = typeof(TEntity);

            if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                Connection.Open();

            var propertyContainer = DapperHelper.ParseProperties(type);
            var sqlIdPairs = DapperHelper.GetSqlPairs(propertyContainer.IdNames);

            var query = $"DELETE FROM [{_tableName}] WHERE {sqlIdPairs}";
            await Connection.ExecuteAsync(query, propertyContainer.IdPairs, commandType: CommandType.Text);
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