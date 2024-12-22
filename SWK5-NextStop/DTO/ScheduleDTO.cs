namespace SWK5_NextStop.DTO;

public class ScheduleDTO
{
    public int ScheduleId { get; set; }
    public int RouteId { get; set; }
    public DateTime Date { get; set; }
    public List<RouteStopScheduleDTO> RouteStopSchedules { get; set; } = new List<RouteStopScheduleDTO>();
}
