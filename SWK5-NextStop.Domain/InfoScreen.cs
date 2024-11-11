namespace NextStop.Data;

public class InfoScreen
{
    public int ScreenId { get; set; }
    public int StopId { get; set; }
    public Stop Stop { get; set; }
    public DateTime DateTime { get; set; }
    public string DisplayedRoutes { get; set; }
}