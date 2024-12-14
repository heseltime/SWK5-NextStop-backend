namespace SWK5_NextStop.DTO;

public class HolidayDTO
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public bool IsSchoolBreak { get; set; }
}
