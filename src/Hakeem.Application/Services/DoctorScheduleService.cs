using Hakeem.Application.DTOs.Schedule;
using Hakeem.Application.Interfaces;
using Hakeem.Domain.Entities;
using Hakeem.Domain.Interfaces;

namespace Hakeem.Application.Services;

public class DoctorScheduleService : IDoctorScheduleService
{
    private readonly IUnitOfWork _unitOfWork;

    public DoctorScheduleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DoctorScheduleResponseDto> AddScheduleAsync(string staffId, CreateDoctorScheduleDto dto)
    {
        var users = await _unitOfWork.Repository<ApplicationUser>().FindAsync(u => u.Id == staffId);
        var user = users.FirstOrDefault();

        if (user == null) throw new Exception("User not found.");

        string doctorId = user.Role == Hakeem.Domain.Enums.UserRole.Doctor ? user.Id : user.AssignedDoctorId!;
        
        if (string.IsNullOrEmpty(doctorId))
            throw new Exception("No doctor assigned to this schedule context.");

        var schedule = new DoctorSchedule
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            DayOfWeek = dto.DayOfWeek,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            SlotDurationMinutes = dto.SlotDurationMinutes
        };

        await _unitOfWork.Repository<DoctorSchedule>().AddAsync(schedule);
        await _unitOfWork.SaveChangesAsync();

        return new DoctorScheduleResponseDto
        {
            Id = schedule.Id,
            DoctorId = schedule.DoctorId,
            DayOfWeek = schedule.DayOfWeek,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime,
            SlotDurationMinutes = schedule.SlotDurationMinutes
        };
    }

    public async Task<IEnumerable<DoctorScheduleResponseDto>> GetDoctorSchedulesAsync(string doctorId)
    {
        var schedules = await _unitOfWork.Repository<DoctorSchedule>().FindAsync(s => s.DoctorId == doctorId);
        
        return schedules.Select(s => new DoctorScheduleResponseDto
        {
            Id = s.Id,
            DoctorId = s.DoctorId,
            DayOfWeek = s.DayOfWeek,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            SlotDurationMinutes = s.SlotDurationMinutes
        });
    }

    public async Task<IEnumerable<DoctorScheduleResponseDto>> GetClinicScheduleAsync(string staffId)
    {
        var users = await _unitOfWork.Repository<ApplicationUser>().FindAsync(u => u.Id == staffId);
        var user = users.FirstOrDefault();

        if (user == null) throw new Exception("User not found.");

        string doctorId = user.Role == Hakeem.Domain.Enums.UserRole.Doctor ? user.Id : user.AssignedDoctorId!;
        
        if (string.IsNullOrEmpty(doctorId))
            throw new Exception("No doctor assigned to this schedule context.");

        return await GetDoctorSchedulesAsync(doctorId);
    }
}
