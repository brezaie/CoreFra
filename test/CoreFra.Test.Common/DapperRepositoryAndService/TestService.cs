using System;
using System.Collections.Generic;
using System.Linq;
using CoreFra.Logging;
using CoreFra.Service;

namespace CoreFra.Test.Common
{
    public class TestService : GenericService<TestEntity, ITestRepository>, ITestService
    {
        private readonly ICustomLogger _logger;


        public TestService(ITestRepository repository, ICustomLogger logger) : base(repository)
        {
            _logger = logger;
        }

        public List<TestEntity> GetByFirstName(string firstName)
        {
            try
            {
                BRule.Assert(!string.IsNullOrEmpty(firstName), "Firstname is invalid");
                var result = Repository.GetByFirstName(firstName);
                return result;
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
                BRule.Assert(age > 0, "Age is invalid");
                var result = Repository.GetMoreThanGivenAge(age);
                return result;
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
                firstNames.ForEach(x => BRule.Assert(!string.IsNullOrEmpty(x), "Firstname is invalid"));
                var result = Repository.GetAllUsersByGivenFirstNames(firstNames);
                return result;

            }
            catch (Exception ex)
            {
                _logger.ErrorException(ex.Message, ex);
                throw;
            }
        }
    }
}