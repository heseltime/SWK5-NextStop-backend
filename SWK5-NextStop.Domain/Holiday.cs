namespace NextStop.Data;

public class Holiday
{
    public int Id  { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public bool IsSchoolBreak { get; set; }
    public int CompanyId { get; set; }
}