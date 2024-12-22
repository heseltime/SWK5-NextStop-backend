namespace SWK5_NextStop.DTO;

public class RouteDTO
{
    public int RouteId { get; set; }
    public string RouteNumber { get; set; }
    public string ValidityPeriod { get; set; }
    public string DayValidity { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } // Optional for additional abstraction
    public List<RouteStopDTO> RouteStops { get; set; } = new List<RouteStopDTO>();
}