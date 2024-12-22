namespace SWK5_NextStop.DAL;

using SWK5_NextStop.Infrastructure;
using NextStop.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;

public class RouteRepository
{
    private readonly AdoTemplate _adoTemplate;

    public RouteRepository(IConnectionFactory connectionFactory)
    {
        _adoTemplate = new AdoTemplate(connectionFactory);
    }

    private Route MapRowToRoute(DbDataReader reader) =>
        new Route
        {
            RouteId = reader.GetInt32(reader.GetOrdinal("route_id")),
            RouteNumber = reader.GetString(reader.GetOrdinal("route_number")),
            ValidityPeriod = reader.GetString(reader.GetOrdinal("validity_period")),
            DayValidity = reader.GetString(reader.GetOrdinal("day_validity")),
            CompanyId = reader.GetInt32(reader.GetOrdinal("company_id"))
        };

    public async Task<IEnumerable<Route>> GetAllRoutesAsync()
    {
        return await _adoTemplate.QueryAsync("SELECT * FROM route", MapRowToRoute);
    }

    public async Task<Route?> GetRouteByIdAsync(int routeId)
    {
        string query = @"
        SELECT * 
        FROM route r
        LEFT JOIN route_stop rs ON r.route_id = rs.route_id
        LEFT JOIN stop s ON rs.stop_id = s.stop_id
        WHERE r.route_id = @routeId";

        return await _adoTemplate.QuerySingleAsync(query, MapRoute, new QueryParameter("@routeId", routeId));
    }
    
    private Route MapRoute(DbDataReader reader)
    {
        var route = new Route
        {
            RouteId = reader.GetInt32(reader.GetOrdinal("route_id")),
            RouteNumber = reader.GetString(reader.GetOrdinal("route_number")),
            ValidityPeriod = reader.GetString(reader.GetOrdinal("validity_period")),
            DayValidity = reader.GetString(reader.GetOrdinal("day_validity")),
            CompanyId = reader.GetInt32(reader.GetOrdinal("company_id")),
            RouteStops = new List<RouteStop>()
        };

        // Map associated stops
        while (reader.Read())
        {
            route.RouteStops.Add(new RouteStop
            {
                StopId = reader.GetInt32(reader.GetOrdinal("stop_id")),
                SequenceNumber = reader.GetInt32(reader.GetOrdinal("sequence_number")),
                Stop = new Stop
                {
                    StopId = reader.GetInt32(reader.GetOrdinal("stop_id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    ShortName = reader.GetString(reader.GetOrdinal("short_name"))
                }
            });
        }

        return route;
    }

    public async Task UpdateRouteAsync(Route route)
    {
        string query = @"
        UPDATE route
        SET route_number = @routeNumber,
            validity_period = @validityPeriod,
            day_validity = @dayValidity,
            company_id = @companyId
        WHERE route_id = @routeId";

        await _adoTemplate.ExecuteAsync(query,
            new QueryParameter("@routeId", route.RouteId),
            new QueryParameter("@routeNumber", route.RouteNumber),
            new QueryParameter("@validityPeriod", route.ValidityPeriod),
            new QueryParameter("@dayValidity", route.DayValidity),
            new QueryParameter("@companyId", route.CompanyId));
    }

    public async Task<Route> CreateRouteAsync(Route route)
    {
        string query = @"
        INSERT INTO route (route_number, validity_period, day_validity, company_id)
        VALUES (@routeNumber, @validityPeriod, @dayValidity, @companyId)
        RETURNING route_id;";

        int generatedId = await _adoTemplate.ExecuteScalarAsync<int>(query,
            new QueryParameter("@routeNumber", route.RouteNumber),
            new QueryParameter("@validityPeriod", route.ValidityPeriod),
            new QueryParameter("@dayValidity", route.DayValidity),
            new QueryParameter("@companyId", route.CompanyId));

        route.RouteId = generatedId;
        return route;
    }

    public async Task<bool> DeleteRouteAsync(int routeId)
    {
        string query = "DELETE FROM route WHERE route_id = @routeId";

        int rowsAffected = await _adoTemplate.ExecuteAsync(query,
            new QueryParameter("@routeId", routeId));

        return rowsAffected > 0;
    }
    
    public async Task<IEnumerable<Route>> SearchRoutesAsync(string? routeNumber, string? validityPeriod)
    {
        string query = "SELECT * FROM route WHERE 1=1";
        var parameters = new List<QueryParameter>();

        if (!string.IsNullOrWhiteSpace(routeNumber))
        {
            query += " AND route_number ILIKE @routeNumber";
            parameters.Add(new QueryParameter("@routeNumber", $"%{routeNumber}%"));
        }

        if (!string.IsNullOrWhiteSpace(validityPeriod))
        {
            query += " AND validity_period ILIKE @validityPeriod";
            parameters.Add(new QueryParameter("@validityPeriod", $"%{validityPeriod}%"));
        }

        return await _adoTemplate.QueryAsync(query, MapRoute, parameters.ToArray());
    }
    
    public async Task AddRouteStopAsync(int routeId, int stopId, int sequenceNumber)
    {
        string query = @"
        INSERT INTO route_stop (route_id, stop_id, sequence_number)
        VALUES (@routeId, @stopId, @sequenceNumber)";

        await _adoTemplate.ExecuteAsync(query,
            new QueryParameter("@routeId", routeId),
            new QueryParameter("@stopId", stopId),
            new QueryParameter("@sequenceNumber", sequenceNumber));
    }
}
