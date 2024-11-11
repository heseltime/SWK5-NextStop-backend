namespace NextStop.Data;

public class CheckIn
{
    public int CheckInId { get; set; }
    public int ScheduleId { get; set; }
    public Schedule Schedule { get; set; }
    public int RouteId { get; set; }
    public Route Route { get; set; }
    public int StopId { get; set; }
    public Stop Stop { get; set; }
    public DateTime DateTime { get; set; }
    public string ApiKey { get; set; }
}