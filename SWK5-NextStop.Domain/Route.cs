namespace NextStop.Data;

public class Route
{
    public int RouteId { get; set; }
    public string RouteNumber { get; set; }
    public string ValidityPeriod { get; set; }
    public string DayValidity { get; set; }
    public int CompanyId { get; set; }
    public Company Company { get; set; }
    public ICollection<RouteStop> RouteStops { get; set; }
}