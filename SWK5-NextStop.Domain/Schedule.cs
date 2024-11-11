namespace NextStop.Data;

public class Schedule
{
    public int ScheduleId { get; set; }
    public int RouteId { get; set; }
    public Route Route { get; set; }
    public int StartStopId { get; set; }
    public Stop StartStop { get; set; }
    public int EndStopId { get; set; }
    public Stop EndStop { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public int Connections { get; set; }
}
