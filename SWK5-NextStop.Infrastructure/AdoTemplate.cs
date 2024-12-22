namespace SWK5_NextStop.Infrastructure;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

public class AdoTemplate
{
    private readonly IConnectionFactory _connectionFactory;

    public AdoTemplate(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string query, Func<DbDataReader, T> map, params QueryParameter[] parameters)
    {
        var results = new List<T>();

        using (var connection = _connectionFactory.CreateConnection())
        {
            await connection.OpenAsync();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                foreach (var param in parameters)
                {
                    var dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = param.Name;
                    dbParameter.Value = param.Value;
                    command.Parameters.Add(dbParameter);
                }

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(map(reader));
                    }
                }
            }
        }

        return results;
    }

    public async Task<T?> QuerySingleAsync<T>(string query, Func<DbDataReader, T> map, params QueryParameter[] parameters) where T : class
    {
        var result = await QueryAsync(query, map, parameters);
        return result.SingleOrDefault();
    }

    public async Task<int> ExecuteAsync(string query, params QueryParameter[] parameters)
    {
        using (var connection = _connectionFactory.CreateConnection())
        {
            await connection.OpenAsync();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                foreach (var param in parameters)
                {
                    var dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = param.Name;
                    dbParameter.Value = param.Value;
                    command.Parameters.Add(dbParameter);
                }

                return await command.ExecuteNonQueryAsync();
            }
        }
    }
    
    public async Task<T> ExecuteScalarAsync<T>(string query, params QueryParameter[] parameters)
    {
        using (var connection = _connectionFactory.CreateConnection())
        {
            await connection.OpenAsync();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                foreach (var param in parameters)
                {
                    var dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = param.Name;
                    dbParameter.Value = param.Value;
                    command.Parameters.Add(dbParameter);
                }

                var result = await command.ExecuteScalarAsync();
                return (T)Convert.ChangeType(result, typeof(T));
            }
        }
    }
}

public record QueryParameter(string Name, object Value);
