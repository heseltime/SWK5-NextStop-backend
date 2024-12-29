using NextStop.Data;
using SWK5_NextStop.DTO;

namespace SWK5_NextStop.Mapper;

public static class ScheduleMapper
{
    public static ScheduleDTO ToDTO(Schedule schedule) =>
        new ScheduleDTO
        {
            ScheduleId = schedule.ScheduleId,
            RouteId = schedule.RouteId,
            Date = schedule.Date,
            RouteStopSchedules = schedule.RouteStopSchedules.Select(rss => new RouteStopScheduleDTO
            {
                StopId = rss.StopId,
                SequenceNumber = rss.SequenceNumber,
                Time = rss.Time
            }).ToList()
        };

    public static Schedule ToDomain(ScheduleDTO dto) =>
        new Schedule
        {
            ScheduleId = dto.ScheduleId,
            RouteId = dto.RouteId,
            Date = dto.Date,
            RouteStopSchedules = dto.RouteStopSchedules.Select(rss => new RouteStopSchedule
            {
                StopId = rss.StopId,
                SequenceNumber = rss.SequenceNumber,
                Time = rss.Time
            }).ToList()
        };
}
