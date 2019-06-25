using System;
using System.Collections.Generic;
using System.Linq;
using CoreFra.Logging;
using CoreFra.Repository;
using CoreFra.Repository.Dapper;
using SqlKata.Execution;

namespace CoreFra.Test.Common
{
    public class TestRepository : DapperGenericRepository<TestEntity>, ITestRepository
    {
        private readonly ICustomLogger _logger;
        public TestRepository(IDapperConnectionFactory dapperConnectionFactory, ICustomLogger logger) : base(dapperConnectionFactory)
        {
            _logger = logger;
        }

        public List<TestEntity> GetByFirstName(string firstName)
        {
            try
            {
                var result = QueryFactory.Query(TableName).Where("FirstName", firstName).Get<TestEntity>();
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorException(ex.Message, ex);
                throw;
            }
        }

        public List<TestEntity> GetMoreThanGivenAge(int age)
        {
            try
            {
                var result = QueryFactory.Query(TableName).Where("Age", ">=", age).Get<TestEntity>();
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorException(ex.Message, ex);
                throw;
            }
        }

        public List<TestEntity> GetAllUsersByGivenFirstNames(List<string> firstNames)
        {
            try
            {
                var result = QueryFactory.Query(TableName).WhereIn("FirstName", firstNames).Get<TestEntity>();
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorException(ex.Message, ex);
                throw;
            }
        }
    }
}