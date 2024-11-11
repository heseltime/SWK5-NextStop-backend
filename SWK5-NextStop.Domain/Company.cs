namespace NextStop.Data;

public class Company
{
    public int CompanyId { get; set; }
    public string Name { get; set; }
    public string AdminUsername { get; set; }
    public string AdminPassword { get; set; }
    public ICollection<Route> Routes { get; set; }
}