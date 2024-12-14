namespace SWK5_NextStop.Mapper;

using NextStop.Data;
using SWK5_NextStop.DTO;

public static class StopMapper
{
    public static StopDTO ToDTO(Stop stop)
    {
        return new StopDTO
        {
            StopId = stop.StopId,
            Name = stop.Name,
            ShortName = stop.ShortName,
            GpsCoordinates = stop.GpsCoordinates
        };
    }

    public static Stop ToDomain(StopDTO stopDTO)
    {
        return new Stop
        {
            StopId = stopDTO.StopId,
            Name = stopDTO.Name,
            ShortName = stopDTO.ShortName,
            GpsCoordinates = stopDTO.GpsCoordinates
        };
    }
}