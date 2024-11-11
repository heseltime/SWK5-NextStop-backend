namespace NextStop.Data;

public class Stop
{
    public int StopId { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string GpsCoordinates { get; set; }
    public ICollection<RouteStop> RouteStops { get; set; }
}