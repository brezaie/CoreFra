using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CoreFra.Domain;
using Dapper;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace CoreFra.Repository.Dapper
{
    public class DapperGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        public IDbConnection Connection { get; }
        public SqlServerCompiler Compiler { get; }
        public QueryFactory QueryFactory { get; }

        #region PrivateProperties

        protected readonly string TableName;

        #endregion

        #region Constructor

        public DapperGenericRepository(IDapperConnectionFactory dapperConnectionFactory)
        {
            Connection = dapperConnectionFactory.Connection;
            Compiler = new SqlServerCompiler();
            QueryFactory = new QueryFactory(Connection, Compiler);

            TableName = typeof(TEntity).Name;
        }

        #endregion

        public IQueryable<TEntity> Queryable()
        {
            try
            {
                var query = $"SELECT * FROM {TableName}";

                if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                    Connection.Open();

                return Connection.Query<TEntity>(query).AsQueryable();
            }
            finally
            {
                Connection.Close();
            }
        }

        public IEnumerable<TEntity> GetAll()
        {
            try
            {
                var query = $"SELECT * FROM {TableName}";

                if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                    Connection.Open();

                return Connection.Query<TEntity>(query).ToList();
            }
            finally
            {
                Connection.Close();
            }
        }

        public PagedCollection<TEntity> GetPagedCollection(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (pageNumber <= 0)
                    pageNumber = 1;
                if (pageSize <= 0)
                    pageSize = 10;

                var lst = QueryFactory.Query(TableName).PaginateAsync<TEntity>(pageNumber, pageSize).Result;

                var res = new PagedCollection<TEntity>
                {
                    List = lst.List,
                    PageNumber = lst.Page,
                    PageSize = lst.PerPage,
                    TotalCount = lst.Count,
                    TotalPages = lst.TotalPages
                };

                return res;
            }
            finally
            {
                QueryFactory.Connection.Close();
            }
        }

        public TEntity FindById(int id)
        {
            try
            {
                var query = $"SELECT * FROM {TableName} WHERE Id = @Id";

                if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                    Connection.Open();

                var res = Connection.QueryFirstOrDefault<TEntity>(query, new {Id = id});
                return res;
            }
            finally
            {
                Connection.Close();
            }
        }

        public async Task<TEntity> FindByIdAsync(int id)
        {
            try
            {
                var query = $"SELECT * FROM {TableName} WHERE Id = @Id";

                if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                    Connection.Open();

                var res = await Connection.QueryFirstOrDefaultAsync<TEntity>(query, new {Id = id});
                return res;
            }
            finally
            {
                Connection.Close();
            }
        }

        public void Insert(TEntity entity)
        {
            try
            {
                if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                    Connection.Open();

                var propertyContainer = DapperHelper.ParseProperties(entity);
                var query =
                    $"INSERT INTO [{TableName}] ({string.Join(", ", propertyContainer.ValueNames)}) " +
                    $"VALUES(@{string.Join(", @", propertyContainer.ValueNames.Select(x => x.Replace("[", "").Replace("]", "")))})";

                Connection.Execute(query, propertyContainer.AllPairs, commandType: CommandType.Text);
            }
            finally
            {
                Connection.Close();
            }
        }

        public async Task InsertAsync(TEntity entity)
        {
            try
            {
                if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                    Connection.Open();

                var propertyContainer = DapperHelper.ParseProperties(entity);
                var query =
                    $"INSERT INTO [{TableName}] ({string.Join(", ", propertyContainer.ValueNames)}) " +
                    $"VALUES(@{string.Join(", @", propertyContainer.ValueNames.Select(x => x.Replace("[", "").Replace("]", "")))})";

                await Connection.ExecuteAsync(query, propertyContainer.AllPairs, commandType: CommandType.Text);
            }
            finally
            {
                Connection.Close();
            }
        }

        public void BulkInsert(IEnumerable<TEntity> entities, SqlTransaction transaction = null,
            int batchSize = 0, int bulkCopyTimeout = 30)
        {
            var type = typeof(TEntity);
            var tableName = DapperHelper.GetTableName(type);
            var tempToBeInserted = $"#{tableName}_TempInsert".Replace(".", string.Empty);

            try
            {
                if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                    Connection.Open();

               
                var allProperties = DapperHelper.TypePropertiesCache(type);
                var keyProperties = DapperHelper.KeyPropertiesCache(type);
                //var computedProperties = ComputedPropertiesCache(type);
                var allPropertiesComputed = allProperties;
                var allPropertiesComputedString = DapperHelper.GetColumnsStringSqlServer(allPropertiesComputed);

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

                if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                    Connection.Open();

                Connection.ExecuteAsync(
                    $@"IF OBJECT_ID('tempdb..#{tempToBeInserted}') IS NOT NULL drop table #{tempToBeInserted}", null,
                    transaction);

                Console.WriteLine(ex);
                throw;
            }
            finally
            {
                Connection.Close();
            }
        }

        public async Task BulkInsertAsync(IEnumerable<TEntity> entities, SqlTransaction transaction = null,
            int batchSize = 0, int bulkCopyTimeout = 30)
        {
            var type = typeof(TEntity);
            var tableName = DapperHelper.GetTableName(type);
            var tempToBeInserted = $"#{tableName}_TempInsert".Replace(".", string.Empty);

            try
            {
                if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                    Connection.Open();

                var allProperties = DapperHelper.TypePropertiesCache(type);
                var keyProperties = DapperHelper.KeyPropertiesCache(type);
                //var computedProperties = ComputedPropertiesCache(type);
                var allPropertiesComputed = allProperties;
                var allPropertiesComputedString = DapperHelper.GetColumnsStringSqlServer(allPropertiesComputed);

                await Connection.ExecuteAsync(
                    $@"IF OBJECT_ID('tempdb..#{tempToBeInserted}') IS NOT NULL drop table #{tempToBeInserted} SELECT TOP 0 {allPropertiesComputedString} INTO {tempToBeInserted} FROM {tableName} target WITH(NOLOCK);",
                    null, transaction);

                using (var bulkCopy =
                    new SqlBulkCopy((SqlConnection) Connection, SqlBulkCopyOptions.Default, transaction))
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
                if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                    Connection.Open();

                await Connection.ExecuteAsync(
                    $@"IF OBJECT_ID('tempdb..#{tempToBeInserted}') IS NOT NULL drop table #{tempToBeInserted}", null,
                    transaction);

                Console.WriteLine(ex);
                throw;
            }
            finally
            {
                Connection.Close();
            }
        }

        public void Update(TEntity entity)
        {
            try
            {
                if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                    Connection.Open();

                var propertyContainer = DapperHelper.ParseProperties(entity);
                var sqlIdPairs = DapperHelper.GetSqlPairs(propertyContainer.IdNames);
                var sqlValuePairs = DapperHelper.GetSqlPairs(propertyContainer.ValueNames);

                var query = $"UPDATE [{TableName}] SET {sqlValuePairs} WHERE {sqlIdPairs}";
                Connection.Execute(query, propertyContainer.AllPairs, commandType: CommandType.Text);
            }
            finally
            {
                Connection.Close();
            }
        }

        public async Task UpdateAsync(TEntity entity)
        {
            try
            {
                if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                    Connection.Open();

                var propertyContainer = DapperHelper.ParseProperties(entity);
                var sqlIdPairs = DapperHelper.GetSqlPairs(propertyContainer.IdNames);
                var sqlValuePairs = DapperHelper.GetSqlPairs(propertyContainer.ValueNames);

                var query = $"UPDATE [{TableName}] SET {sqlValuePairs} WHERE {sqlIdPairs}";
                await Connection.ExecuteAsync(query, propertyContainer.AllPairs, commandType: CommandType.Text);
            }
            finally
            {
                Connection.Close();
            }
        }

        public void Delete(int id)
        {
            try
            {
                var type = typeof(TEntity);

                if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                    Connection.Open();

                var propertyContainer = DapperHelper.ParseProperties(type);
                var sqlIdPairs = DapperHelper.GetSqlPairs(propertyContainer.IdNames);

                var query = $"DELETE FROM [{TableName}] WHERE {sqlIdPairs}";
                Connection.Execute(query, propertyContainer.IdPairs, commandType: CommandType.Text);
            }
            finally
            {
                Connection.Close();
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var type = typeof(TEntity);

                if (Connection.State == ConnectionState.Closed || Connection.State == ConnectionState.Broken)
                    Connection.Open();

                var propertyContainer = DapperHelper.ParseProperties(type);
                var sqlIdPairs = DapperHelper.GetSqlPairs(propertyContainer.IdNames);

                var query = $"DELETE FROM [{TableName}] WHERE {sqlIdPairs}";
                await Connection.ExecuteAsync(query, propertyContainer.IdPairs, commandType: CommandType.Text);
            }
            finally
            {
                Connection.Close();
            }
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