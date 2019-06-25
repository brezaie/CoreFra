using System.Collections.Generic;
using CoreFra.Service;

namespace CoreFra.Test.Common
{
    public interface ITestService : IGenericService<TestEntity>
    {
        List<TestEntity> GetByFirstName(string firstName);
        List<TestEntity> GetMoreThanGivenAge(int age);
        List<TestEntity> GetAllUsersByGivenFirstNames(List<string> firstNames);
    }
}