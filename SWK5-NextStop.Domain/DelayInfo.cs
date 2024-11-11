namespace NextStop.Data;

public class DelayInfo
{
    public int DelayId { get; set; }
    public int ScheduleId { get; set; }
    public Schedule Schedule { get; set; }
    public int TimeDifference { get; set; }  // in minutes
    public DateTime UpdatedTime { get; set; }
}