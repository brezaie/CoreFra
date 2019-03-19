using System.Data;

namespace CoreFra.Repository
{
    public interface IDapperConnectionFactory
    {
        IDbConnection Connection { get; }
    }
}