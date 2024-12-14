namespace SWK5_NextStop.Mapper;

using NextStop.Data;
using SWK5_NextStop.DTO;

public static class HolidayMapper
{
    public static HolidayDTO ToDTO(Holiday holiday)
    {
        return new HolidayDTO
        {
            Id = holiday.Id,
            Description = holiday.Description,
            Date = holiday.Date,
            IsSchoolBreak = holiday.IsSchoolBreak
        };
    }

    public static Holiday ToDomain(HolidayDTO holidayDTO)
    {
        return new Holiday
        {
            Id = holidayDTO.Id,
            Description = holidayDTO.Description,
            Date = holidayDTO.Date,
            IsSchoolBreak = holidayDTO.IsSchoolBreak
        };
    }
}