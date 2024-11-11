namespace SWK5_NextStop.Infrastructure;

using Npgsql;
using System.Data.Common;

public class ConnectionFactory : IConnectionFactory
{
    private readonly string _connectionString;

    public ConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
