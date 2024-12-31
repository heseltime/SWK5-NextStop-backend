namespace SWK5_NextStop.DAL;

using SWK5_NextStop.Infrastructure;
using NextStop.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;

public class ScheduleRepository
{
    private readonly AdoTemplate _adoTemplate;

    public ScheduleRepository(IConnectionFactory connectionFactory)
    {
        _adoTemplate = new AdoTemplate(connectionFactory);
    }

    private Schedule MapRowToSchedule(DbDataReader reader) =>
        new Schedule
        {
            ScheduleId = reader.GetInt32(reader.GetOrdinal("schedule_id")),
            RouteId = reader.GetInt32(reader.GetOrdinal("route_id")),
            Date = reader.GetDateTime(reader.GetOrdinal("date")),
            ValidityStart = reader.GetDateTime(reader.GetOrdinal("validity_start")),
            ValidityStop = reader.GetDateTime(reader.GetOrdinal("validity_stop")),
        };

    private RouteStopSchedule MapRowToRouteStopSchedule(DbDataReader reader) =>
        new RouteStopSchedule
        {
            ScheduleId = reader.GetInt32(reader.GetOrdinal("schedule_id")),
            StopId = reader.GetInt32(reader.GetOrdinal("stop_id")),
            SequenceNumber = reader.GetInt32(reader.GetOrdinal("sequence_number")),
            Time = reader.GetFieldValue<TimeOnly>(reader.GetOrdinal("time"))
        };

    public async Task<Schedule> CreateScheduleAsync(Schedule schedule)
    {
        string query = @"
            INSERT INTO schedule (route_id, date, validity_start, validity_stop)
            VALUES (@routeId, @date, @validityStart, @validityStop)
            RETURNING schedule_id;";

        int generatedId = await _adoTemplate.ExecuteScalarAsync<int>(query,
            new QueryParameter("@routeId", schedule.RouteId),
            new QueryParameter("@date", schedule.Date),
            new QueryParameter("@validityStart", schedule.ValidityStart),
            new QueryParameter("@validityStop", schedule.ValidityStop));

        schedule.ScheduleId = generatedId;
        return schedule;
    }

    public async Task AddRouteStopScheduleAsync(RouteStopSchedule routeStopSchedule)
    {
        string query = @"
            INSERT INTO route_stop_schedule (schedule_id, stop_id, sequence_number, time)
            VALUES (@scheduleId, @stopId, @sequenceNumber, @time);";

        await _adoTemplate.ExecuteAsync(query,
            new QueryParameter("@scheduleId", routeStopSchedule.ScheduleId),
            new QueryParameter("@stopId", routeStopSchedule.StopId),
            new QueryParameter("@sequenceNumber", routeStopSchedule.SequenceNumber),
            new QueryParameter("@time", routeStopSchedule.Time));
    }

    public async Task<Schedule?> GetScheduleByIdAsync(int scheduleId)
    {
        string scheduleQuery = "SELECT * FROM schedule WHERE schedule_id = @scheduleId";
        string routeStopQuery = "SELECT * FROM route_stop_schedule WHERE schedule_id = @scheduleId";

        var schedule = await _adoTemplate.QuerySingleAsync(scheduleQuery, MapRowToSchedule, new QueryParameter("@scheduleId", scheduleId));
        if (schedule == null) return null;

        var routeStops = await _adoTemplate.QueryAsync(routeStopQuery, MapRowToRouteStopSchedule, new QueryParameter("@scheduleId", scheduleId));
        schedule.RouteStopSchedules = new List<RouteStopSchedule>(routeStops);

        return schedule;
    }

    public async Task<IEnumerable<Schedule>> GetAllSchedulesAsync()
    {
        string query = "SELECT * FROM schedule";

        return await _adoTemplate.QueryAsync(query, MapRowToSchedule);
    }
    
    public async Task<IEnumerable<Schedule>> FindSchedulesBetweenStopsAsync(int startStopId, int endStopId)
    {
        string query = @"
        SELECT DISTINCT s.*
        FROM schedule s
        JOIN route_stop_schedule rss_start ON s.schedule_id = rss_start.schedule_id
        JOIN route_stop_schedule rss_end ON s.schedule_id = rss_end.schedule_id
        WHERE rss_start.stop_id = @startStopId
          AND rss_end.stop_id = @endStopId
          AND rss_start.sequence_number < rss_end.sequence_number";

        return await _adoTemplate.QueryAsync(query, MapRowToSchedule,
            new QueryParameter("@startStopId", startStopId),
            new QueryParameter("@endStopId", endStopId));
    }
    
    public async Task<IEnumerable<Schedule>> FindSchedulesByTimeAsync(int startStopId, int endStopId, TimeOnly startTime, TimeOnly arrivalTime)
    {
        string query = @"
        ";

        return await _adoTemplate.QueryAsync(query, MapRowToSchedule,
            new QueryParameter("@startStopId", startStopId),
            new QueryParameter("@endStopId", endStopId),
            new QueryParameter("@startTime", startTime),
            new QueryParameter("@arrivalTime", arrivalTime));
    }
    
    public async Task<IEnumerable<Schedule>> GetNextConnectionsAsync(int stopId, DateTime dateTime, int count)
    {
        string query = @"
            SELECT s.*, (s.date + rss.time) AS stop_time
            FROM schedule s
            JOIN route_stop_schedule rss ON s.schedule_id = rss.schedule_id
            WHERE rss.stop_id = @stopId
              AND (s.date > @date OR (s.date = @date AND rss.time > @time))
            ORDER BY stop_time
            LIMIT @count;";

        return await _adoTemplate.QueryAsync(query, reader =>
            {
                var schedule = MapRowToSchedule(reader);
                return schedule;
            },
            new QueryParameter("@stopId", stopId),
            new QueryParameter("@date", dateTime.Date),
            new QueryParameter("@time", dateTime.TimeOfDay),
            new QueryParameter("@count", count));
    }

    public async Task<IEnumerable<RouteStopSchedule>> GetRemainingStopsAsync(int scheduleId, int stopId)
    {
        string query = @"
    SELECT rss.*, (CURRENT_DATE + rss.time) AS stop_time
    FROM route_stop_schedule rss
    WHERE rss.schedule_id = @scheduleId
      AND rss.sequence_number >= (
          SELECT sequence_number
          FROM route_stop_schedule
          WHERE stop_id = @stopId AND schedule_id = @scheduleId
          LIMIT 1
      )
    ORDER BY rss.sequence_number;";

        return await _adoTemplate.QueryAsync(query, reader =>
            {
                return new RouteStopSchedule
                {
                    RouteStopId = reader.GetInt32(reader.GetOrdinal("route_stop_id")),
                    ScheduleId = reader.GetInt32(reader.GetOrdinal("schedule_id")),
                    StopId = reader.GetInt32(reader.GetOrdinal("stop_id")),
                    SequenceNumber = reader.GetInt32(reader.GetOrdinal("sequence_number")),
                    Time = TimeOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("stop_time")))
                };
            },
            new QueryParameter("@scheduleId", scheduleId),
            new QueryParameter("@stopId", stopId));
    }
    
    public async Task<IEnumerable<RouteStopSchedule>> GetStopsAsync(int scheduleId)
    {
        string query = @"
    SELECT rss.*, (CURRENT_DATE + rss.time) AS stop_time
    FROM route_stop_schedule rss
    WHERE rss.schedule_id = @scheduleId
    ORDER BY rss.sequence_number;";

        return await _adoTemplate.QueryAsync(query, reader =>
            {
                return new RouteStopSchedule
                {
                    RouteStopId = reader.GetInt32(reader.GetOrdinal("route_stop_id")),
                    ScheduleId = reader.GetInt32(reader.GetOrdinal("schedule_id")),
                    StopId = reader.GetInt32(reader.GetOrdinal("stop_id")),
                    SequenceNumber = reader.GetInt32(reader.GetOrdinal("sequence_number")),
                    Time = TimeOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("stop_time")))
                };
            },
            new QueryParameter("@scheduleId", scheduleId));
    }

    
    public async Task<int> SaveCheckInAsync(CheckIn checkIn)
    {
        string query = @"
            INSERT INTO check_in (schedule_id, route_id, stop_id, datetime, api_key)
            VALUES (@scheduleId, @routeId, @stopId, @dateTime, @apiKey)
            RETURNING check_in_id;";

        return await _adoTemplate.ExecuteScalarAsync<int>(query,
            new QueryParameter("@scheduleId", checkIn.ScheduleId),
            new QueryParameter("@routeId", checkIn.RouteId),
            new QueryParameter("@stopId", checkIn.StopId),
            new QueryParameter("@dateTime", checkIn.DateTime),
            new QueryParameter("@apiKey", checkIn.ApiKey));
    }

    public async Task<bool> ValidatePlausibilityAsync(int scheduleId, int routeId, int stopId)
    {
        string query = @"
            SELECT COUNT(*) 
            FROM schedule s
            JOIN route_stop_schedule rss ON s.schedule_id = rss.schedule_id
            WHERE s.schedule_id = @scheduleId AND rss.route_id = @routeId AND rss.stop_id = @stopId;";

        int count = await _adoTemplate.ExecuteScalarAsync<int>(query,
            new QueryParameter("@scheduleId", scheduleId),
            new QueryParameter("@routeId", routeId),
            new QueryParameter("@stopId", stopId));

        return count > 0;
    }
    
    public async Task UpdateScheduleAsync(Schedule schedule)
    {
        string updateScheduleQuery = @"
        UPDATE schedule
        SET route_id = @routeId,
            date = @date,
            validity_start = @validityStart,
            validity_stop = @validityStop
        WHERE schedule_id = @scheduleId;";

        await _adoTemplate.ExecuteAsync(updateScheduleQuery,
            new QueryParameter("@routeId", schedule.RouteId),
            new QueryParameter("@date", schedule.Date),
            new QueryParameter("@validityStart", schedule.ValidityStart),
            new QueryParameter("@validityStop", schedule.ValidityStop),
            new QueryParameter("@scheduleId", schedule.ScheduleId));

        // Remove existing route stop schedules for the schedule
        string deleteRouteStopSchedulesQuery = @"
        DELETE FROM route_stop_schedule
        WHERE schedule_id = @scheduleId;";

        await _adoTemplate.ExecuteAsync(deleteRouteStopSchedulesQuery,
            new QueryParameter("@scheduleId", schedule.ScheduleId));

        // Add the new route stop schedules
        string insertRouteStopSchedulesQuery = @"
        INSERT INTO route_stop_schedule (schedule_id, stop_id, sequence_number, time)
        VALUES (@scheduleId, @stopId, @sequenceNumber, @time);";

        foreach (var routeStop in schedule.RouteStopSchedules)
        {
            await _adoTemplate.ExecuteAsync(insertRouteStopSchedulesQuery,
                new QueryParameter("@scheduleId", schedule.ScheduleId),
                new QueryParameter("@stopId", routeStop.StopId),
                new QueryParameter("@sequenceNumber", routeStop.SequenceNumber),
                new QueryParameter("@time", routeStop.Time));
        }
    }
}
