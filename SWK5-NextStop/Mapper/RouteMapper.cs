namespace SWK5_NextStop.Mapper;

using NextStop.Data;
using SWK5_NextStop.DTO;

public static class RouteMapper
{
    public static RouteDTO ToDTO(Route route)
    {
        return new RouteDTO
        {
            RouteId = route.RouteId,
            RouteNumber = route.RouteNumber,
            ValidityPeriod = route.ValidityPeriod,
            DayValidity = route.DayValidity
        };
    }

    public static Route ToDomain(RouteDTO routeDTO)
    {
        return new Route
        {
            RouteId = routeDTO.RouteId,
            RouteNumber = routeDTO.RouteNumber,
            ValidityPeriod = routeDTO.ValidityPeriod,
            DayValidity = routeDTO.DayValidity
        };
    }
}