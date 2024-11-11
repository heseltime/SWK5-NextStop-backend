using System.Data.Common;

namespace SWK5_NextStop.Infrastructure;

public interface IConnectionFactory
{
    DbConnection CreateConnection();
}