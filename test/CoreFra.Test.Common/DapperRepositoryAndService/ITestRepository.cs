using System.Collections.Generic;
using CoreFra.Repository;

namespace CoreFra.Test.Common
{
    public interface ITestRepository : IGenericRepository<TestEntity>
    {
        List<TestEntity> GetByFirstName(string firstName);
        List<TestEntity> GetMoreThanGivenAge(int age);
        List<TestEntity> GetAllUsersByGivenFirstNames(List<string> firstNames);
    }
}