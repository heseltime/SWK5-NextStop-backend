using System.Data;
using System.Data.Common;

namespace SWK5_NextStop.Test;

public class MockDbConnection : DbConnection
{
    private readonly DbCommand _command;

    public MockDbConnection(DbCommand command)
    {
        _command = command;
    }
    
    protected override DbCommand CreateDbCommand()
    {
        return _command;
    }

    // Implement other abstract members with default behavior or throw NotImplementedException
    public override string ConnectionString { get; set; }
    public override string Database => throw new NotImplementedException();
    public override string DataSource => throw new NotImplementedException();
    public override string ServerVersion => throw new NotImplementedException();
    public override ConnectionState State => throw new NotImplementedException();
    public override void ChangeDatabase(string databaseName) => throw new NotImplementedException();
    public override void Close() => throw new NotImplementedException();
    public override void Open() => throw new NotImplementedException();
    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotImplementedException();
}