using System.Data;
using System.Data.SqlClient;
using CoreFra.Repository;
using Microsoft.Extensions.Configuration;

namespace CoreFra.Test.Common
{
    public class SampleDapperConnectionFactory : IDapperConnectionFactory
    {
        private readonly string _connectionString;
        public SampleDapperConnectionFactory(string connectioString)
        {
            _connectionString = connectioString; //configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection Connection => new SqlConnection(_connectionString);
    }
}