namespace SWK5_NextStop.Mapper;

using NextStop.Data;
using SWK5_NextStop.DTO;

public static class CheckInMapper
{
    /// <summary>
    /// Maps a CheckInDTO to a CheckIn domain model.
    /// </summary>
    /// <param name="dto">The CheckInDTO object.</param>
    /// <returns>The CheckIn domain model.</returns>
    public static CheckIn ToDomain(CheckInDTO dto) =>
        new CheckIn
        {
            ScheduleId = dto.ScheduleId,
            RouteId = dto.RouteId,
            StopId = dto.StopId,
            DateTime = dto.DateTime,
            ApiKey = dto.ApiKey
        };

    /// <summary>
    /// Maps a CheckIn domain model to a CheckInDTO.
    /// </summary>
    /// <param name="checkIn">The CheckIn domain model.</param>
    /// <returns>The CheckInDTO.</returns>
    public static CheckInDTO ToDTO(CheckIn checkIn) =>
        new CheckInDTO
        {
            ScheduleId = checkIn.ScheduleId,
            RouteId = checkIn.RouteId,
            StopId = checkIn.StopId,
            DateTime = checkIn.DateTime,
            ApiKey = checkIn.ApiKey
        };
}
