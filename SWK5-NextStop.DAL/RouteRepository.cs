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
        return await _adoTemplate.QuerySingleAsync(
            "SELECT * FROM route WHERE route_id = @routeId",
            MapRowToRoute,
            new QueryParameter("@routeId", routeId)
        );
    }

    public async Task<bool> UpdateRouteAsync(Route route)
    {
        string query = @"
            UPDATE route 
            SET route_number = @routeNumber, validity_period = @validityPeriod, 
                day_validity = @dayValidity, company_id = @companyId 
            WHERE route_id = @routeId";

        int rowsAffected = await _adoTemplate.ExecuteAsync(query,
            new QueryParameter("@routeId", route.RouteId),
            new QueryParameter("@routeNumber", route.RouteNumber),
            new QueryParameter("@validityPeriod", route.ValidityPeriod),
            new QueryParameter("@dayValidity", route.DayValidity),
            new QueryParameter("@companyId", route.CompanyId));

        return rowsAffected > 0;
    }

    public async Task<bool> CreateRouteAsync(Route route)
    {
        string query = @"
        INSERT INTO route (route_number, validity_period, day_validity, company_id) 
        VALUES (@routeNumber, @validityPeriod, @dayValidity, @companyId)";

        int rowsAffected = await _adoTemplate.ExecuteAsync(query,
            new QueryParameter("@routeNumber", route.RouteNumber),
            new QueryParameter("@validityPeriod", route.ValidityPeriod),
            new QueryParameter("@dayValidity", route.DayValidity),
            new QueryParameter("@companyId", route.CompanyId));

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteRouteAsync(int routeId)
    {
        string query = "DELETE FROM route WHERE route_id = @routeId";

        int rowsAffected = await _adoTemplate.ExecuteAsync(query,
            new QueryParameter("@routeId", routeId));

        return rowsAffected > 0;
    }
}
