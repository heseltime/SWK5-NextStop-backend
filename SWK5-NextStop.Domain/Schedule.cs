namespace NextStop.Data;

public class Schedule
{
    public int ScheduleId { get; set; }
    public int RouteId { get; set; }
    public Route Route { get; set; }
    public DateTime ValidityStart { get; set; }
    public DateTime ValidityStop { get; set; }
    public DateTime Date { get; set; }
    public ICollection<RouteStopSchedule> RouteStopSchedules { get; set; } = new List<RouteStopSchedule>();
}

public class RouteStopSchedule
{
    public int RouteStopId { get; set; } // Composite key with ScheduleId
    public int ScheduleId { get; set; }
    public Schedule Schedule { get; set; }
    public int StopId { get; set; }
    public Stop Stop { get; set; }
    public int SequenceNumber { get; set; } // Order of the stop in the route
    public TimeOnly Time { get; set; } // The time to leave at this stop
}