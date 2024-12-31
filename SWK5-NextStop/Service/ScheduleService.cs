using NextStop.Data;

namespace SWK5_NextStop.Service;

using SWK5_NextStop.DAL;
using SWK5_NextStop.DTO;
using SWK5_NextStop.Mapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ScheduleService
{
    private readonly ScheduleRepository _scheduleRepository;

    private readonly IApiKeyValidator _apiKeyValidator;

    public ScheduleService(ScheduleRepository scheduleRepository, IApiKeyValidator apiKeyValidator)
    {
        _scheduleRepository = scheduleRepository;
        _apiKeyValidator = apiKeyValidator;
    }

    public async Task<ScheduleDTO> AddScheduleAsync(ScheduleDTO scheduleDto)
    {
        if (scheduleDto == null)
        {
            throw new ArgumentNullException(nameof(scheduleDto), "Schedule DTO cannot be null.");
        }

        // Map DTO to domain entity
        var scheduleEntity = ScheduleMapper.ToDomain(scheduleDto);

        // Save schedule
        var createdSchedule = await _scheduleRepository.CreateScheduleAsync(scheduleEntity);

        // Save each RouteStopSchedule
        foreach (var routeStopScheduleDto in scheduleDto.RouteStopSchedules)
        {
            var routeStopScheduleEntity = new RouteStopSchedule
            {
                ScheduleId = createdSchedule.ScheduleId,
                StopId = routeStopScheduleDto.StopId,
                SequenceNumber = routeStopScheduleDto.SequenceNumber,
                Time = routeStopScheduleDto.Time
            };

            await _scheduleRepository.AddRouteStopScheduleAsync(routeStopScheduleEntity);
        }

        // Map back to DTO and return
        return ScheduleMapper.ToDTO(createdSchedule);
    }

    public async Task<ScheduleDTO?> GetScheduleByIdAsync(int scheduleId)
    {
        var schedule = await _scheduleRepository.GetScheduleByIdAsync(scheduleId);
        return schedule != null ? ScheduleMapper.ToDTO(schedule) : null;
    }

    public async Task<IEnumerable<ScheduleDTO>> GetAllSchedulesAsync()
    {
        var schedules = await _scheduleRepository.GetAllSchedulesAsync();
        var results = new List<ScheduleDTO>();
        foreach (var schedule in schedules)
        {
            var remainingStops = await _scheduleRepository.GetStopsAsync(schedule.ScheduleId);
            var newScheduleDto = new ScheduleDTO()
            {
                ScheduleId = schedule.ScheduleId,
                RouteId = schedule.RouteId,
                Date = schedule.Date,
                RouteStopSchedules = new List<RouteStopScheduleDTO>()
            };

            foreach (var remainingStop in remainingStops)
            {
                newScheduleDto.RouteStopSchedules.Add(new RouteStopScheduleDTO()
                {
                    StopId = remainingStop.StopId,
                    SequenceNumber = remainingStop.SequenceNumber,
                    Time = remainingStop.Time,
                });
            }
            
            results.Add(newScheduleDto);
        }

        return results;
    }
    
    public async Task<IEnumerable<ScheduleDTO>> FindSchedulesBetweenStopsAsync(int startStopId, int endStopId)
    {
        var schedules = await _scheduleRepository.FindSchedulesBetweenStopsAsync(startStopId, endStopId);

        // Map to DTOs
        return schedules.Select(ScheduleMapper.ToDTO);
    }

    public async Task<IEnumerable<ScheduleDTO>> FindSchedulesByTimeAsync(int startStopId, int endStopId, TimeOnly startTime, TimeOnly arrivalTime)
    {
        var schedules = await _scheduleRepository.FindSchedulesByTimeAsync(startStopId, endStopId, startTime, arrivalTime);

        // Map to DTOs
        return schedules.Select(ScheduleMapper.ToDTO);
    }
    
    public async Task<IEnumerable<ScheduleDTO>> GetNextConnectionsAsync(int stopId, DateTime dateTime, int count)
    {
        var schedules = await _scheduleRepository.GetNextConnectionsAsync(stopId, dateTime, count);

        // Map the result to DTOs
        //return schedules.Select(ScheduleMapper.ToDTO);
        var results = new List<ScheduleDTO>();
        foreach (var schedule in schedules)
        {
            var remainingStops = await _scheduleRepository.GetRemainingStopsAsync(schedule.ScheduleId, stopId);
            var newScheduleDto = new ScheduleDTO()
            {
                ScheduleId = schedule.ScheduleId,
                RouteId = schedule.RouteId,
                Date = schedule.Date,
                RouteStopSchedules = new List<RouteStopScheduleDTO>()
            };

            foreach (var remainingStop in remainingStops)
            {
                newScheduleDto.RouteStopSchedules.Add(new RouteStopScheduleDTO()
                {
                    StopId = remainingStop.StopId,
                    SequenceNumber = remainingStop.SequenceNumber,
                    Time = remainingStop.Time,
                });
            }
            
            results.Add(newScheduleDto);
        }

        return results;
    }
    
    public async Task<int> HandleCheckInAsync(CheckInDTO checkInDto)
    {
        if (checkInDto == null)
        {
            throw new ArgumentNullException(nameof(checkInDto), "Check-in data cannot be null.");
        }

        // API Key Verification
        if (!_apiKeyValidator.ValidateApiKey(checkInDto.ApiKey))
        {
            throw new UnauthorizedAccessException("Invalid API key.");
        }

        // Plausibility Check
        bool isPlausible = await _scheduleRepository.ValidatePlausibilityAsync(
            checkInDto.ScheduleId,
            checkInDto.RouteId,
            checkInDto.StopId);

        if (!isPlausible)
        {
            throw new ArgumentException("Check-in data is not plausible.");
        }

        // Save Check-In
        var checkIn = CheckInMapper.ToDomain(checkInDto);
        return await _scheduleRepository.SaveCheckInAsync(checkIn);
    }
    
    public async Task UpdateScheduleAsync(int id, ScheduleDTO scheduleDto)
    {
        if (scheduleDto == null || id != scheduleDto.ScheduleId)
        {
            throw new ArgumentException("Schedule ID mismatch or invalid schedule data.");
        }

        // Fetch the existing schedule
        var existingSchedule = await _scheduleRepository.GetScheduleByIdAsync(id);

        if (existingSchedule == null)
        {
            throw new KeyNotFoundException($"Schedule with ID {id} not found.");
        }

        // Map updated fields from DTO to the existing schedule
        existingSchedule.RouteId = scheduleDto.RouteId;
        existingSchedule.Date = scheduleDto.Date;
        existingSchedule.ValidityStart = scheduleDto.ValidityStart;
        existingSchedule.ValidityStop = scheduleDto.ValidityStop;

        // Update route stop schedules
        existingSchedule.RouteStopSchedules.Clear();
        foreach (var stop in scheduleDto.RouteStopSchedules)
        {
            existingSchedule.RouteStopSchedules.Add(new RouteStopSchedule
            {
                StopId = stop.StopId,
                SequenceNumber = stop.SequenceNumber,
                Time = new TimeOnly(stop.Time.Hour, stop.Time.Minute)
            });
        }

        // Save the updated schedule in the repository
        await _scheduleRepository.UpdateScheduleAsync(existingSchedule);
    }
}