namespace SWK5_NextStop.DAL;

using SWK5_NextStop.Infrastructure;
using NextStop.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;

public class StopRepository
{
    private readonly AdoTemplate _adoTemplate;

    public StopRepository(IConnectionFactory connectionFactory)
    {
        _adoTemplate = new AdoTemplate(connectionFactory);
    }

    private Stop MapRowToStop(DbDataReader reader) =>
        new Stop
        {
            StopId = reader.GetInt32(reader.GetOrdinal("stop_id")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            ShortName = reader.GetString(reader.GetOrdinal("short_name")),
            GpsCoordinates = reader.GetString(reader.GetOrdinal("gps_coordinates"))
        };

    public async Task<IEnumerable<Stop>> GetAllStopsAsync()
    {
        return await _adoTemplate.QueryAsync("SELECT * FROM stop", MapRowToStop);
    }

    public async Task<Stop?> GetStopByIdAsync(int stopId)
    {
        return await _adoTemplate.QuerySingleAsync(
            "SELECT * FROM stop WHERE stop_id = @stopId",
            MapRowToStop,
            new QueryParameter("@stopId", stopId)
        );
    }
    
    public async Task<Stop> CreateStopAsync(Stop stop)
    {
        string query = @"
        INSERT INTO stop (name, short_name, gps_coordinates) 
        VALUES (@name, @short_name, @gps_coordinates)
        RETURNING stop_id;";

        // Execute the query and retrieve the generated stop_id
        int generatedId = await _adoTemplate.ExecuteScalarAsync<int>(query,
            new QueryParameter("@name", stop.Name),
            new QueryParameter("@short_name", stop.ShortName),
            new QueryParameter("@gps_coordinates", stop.GpsCoordinates));

        // Assign the generated ID to the Stop object
        stop.StopId = generatedId;

        return stop;
    }
    
    public async Task<bool> UpdateStopAsync(Stop stop)
    {
        string query = @"
            UPDATE stop 
            SET name = @name, short_name = @shortName, gps_coordinates = @gpsCoordinates 
            WHERE stop_id = @stopId";

        int rowsAffected = await _adoTemplate.ExecuteAsync(query,
            new QueryParameter("@stopId", stop.StopId),
            new QueryParameter("@name", stop.Name),
            new QueryParameter("@shortName", stop.ShortName),
            new QueryParameter("@gpsCoordinates", stop.GpsCoordinates));

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteStopAsync(int stopId)
    {
        string query = "DELETE FROM stop WHERE stop_id = @stopId";

        int rowsAffected = await _adoTemplate.ExecuteAsync(query,
            new QueryParameter("@stopId", stopId));

        return rowsAffected > 0;
    }
}
