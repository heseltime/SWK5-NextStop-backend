namespace SWK5_NextStop.Mapper;

using NextStop.Data;
using SWK5_NextStop.DTO;

public static class RouteMapper
{
    public static RouteDTO ToDTO(Route route) =>
        new RouteDTO
        {
            RouteId = route.RouteId,
            RouteNumber = route.RouteNumber,
            ValidityPeriod = route.ValidityPeriod,
            DayValidity = route.DayValidity,
            CompanyId = route.CompanyId,
            CompanyName = route.Company?.Name,
            RouteStops = route.RouteStops?.Select(rs => new RouteStopDTO
            {
                StopId = rs.StopId,
                SequenceNumber = rs.SequenceNumber
            }).ToList() ?? new List<RouteStopDTO>()
        };

    public static Route ToDomain(RouteDTO dto) =>
        new Route
        {
            RouteId = dto.RouteId,
            RouteNumber = dto.RouteNumber,
            ValidityPeriod = dto.ValidityPeriod,
            DayValidity = dto.DayValidity,
            CompanyId = dto.CompanyId,
            RouteStops = dto.RouteStops.Select(rs => new RouteStop
            {
                StopId = rs.StopId,
                SequenceNumber = rs.SequenceNumber
            }).ToList()
        };
}