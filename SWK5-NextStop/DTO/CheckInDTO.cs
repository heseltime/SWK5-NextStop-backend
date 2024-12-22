namespace SWK5_NextStop.DTO;

public class CheckInDTO
{
    public int ScheduleId { get; set; }
    public int RouteId { get; set; }
    public int StopId { get; set; }
    public DateTime DateTime { get; set; }
    public string ApiKey { get; set; }
}
