namespace SWK5_NextStop.DAL;

using SWK5_NextStop.Infrastructure;
using NextStop.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;

public class HolidayRepository
{
    private readonly AdoTemplate _adoTemplate;

    public HolidayRepository(IConnectionFactory connectionFactory)
    {
        _adoTemplate = new AdoTemplate(connectionFactory);
    }

    private Holiday MapRowToHoliday(DbDataReader reader) =>
        new Holiday
        {
            Id = reader.GetInt32(reader.GetOrdinal("holiday_id")),
            Date = reader.GetDateTime(reader.GetOrdinal("date")),
            Description = reader.GetString(reader.GetOrdinal("description")),
            IsSchoolBreak = reader.GetBoolean(reader.GetOrdinal("is_school_break"))
        };

    public async Task<IEnumerable<Holiday>> GetAllHolidaysAsync()
    {
        return await _adoTemplate.QueryAsync("SELECT * FROM holiday", MapRowToHoliday);
    }

    public async Task<Holiday?> GetHolidayByIdAsync(int holidayId)
    {
        return await _adoTemplate.QuerySingleAsync(
            "SELECT * FROM holiday WHERE holiday_id = @holidayId",
            MapRowToHoliday,
            new QueryParameter("@holidayId", holidayId)
        );
    }

    public async Task<bool> CreateHolidayAsync(Holiday holiday)
    {
        string query = @"
            INSERT INTO holiday (date, description, is_school_break, company_id) 
            VALUES (@date, @description, @isSchoolBreak, @companyId)";

        int rowsAffected = await _adoTemplate.ExecuteAsync(query,
            new QueryParameter("@date", holiday.Date),
            new QueryParameter("@description", holiday.Description),
            new QueryParameter("@isSchoolBreak", holiday.IsSchoolBreak),
            new QueryParameter("@companyId", holiday.CompanyId));

        return rowsAffected > 0;
    }

    public async Task<bool> UpdateHolidayAsync(Holiday holiday)
    {
        string query = @"
            UPDATE holiday 
            SET date = @date, description = @description, is_school_break = @isSchoolBreak 
            WHERE holiday_id = @holidayId";

        int rowsAffected = await _adoTemplate.ExecuteAsync(query,
            new QueryParameter("@holidayId", holiday.Id),
            new QueryParameter("@date", holiday.Date),
            new QueryParameter("@description", holiday.Description),
            new QueryParameter("@isSchoolBreak", holiday.IsSchoolBreak));

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteHolidayAsync(int holidayId)
    {
        string query = "DELETE FROM holiday WHERE holiday_id = @holidayId";

        int rowsAffected = await _adoTemplate.ExecuteAsync(query,
            new QueryParameter("@holidayId", holidayId));

        return rowsAffected > 0;
    }
}
