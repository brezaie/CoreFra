using System;
using CoreFra.Repository;

namespace CoreFra.Test.Common
{
    public class TestEntity
    {
        [DapperKey]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
    }
}