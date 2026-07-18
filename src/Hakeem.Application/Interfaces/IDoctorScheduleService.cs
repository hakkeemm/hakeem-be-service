using Hakeem.Application.DTOs.Schedule;

namespace Hakeem.Application.Interfaces;

public interface IDoctorScheduleService
{
    Task<DoctorScheduleResponseDto> AddScheduleAsync(string assistantId, CreateDoctorScheduleDto dto);
    Task<IEnumerable<DoctorScheduleResponseDto>> GetDoctorSchedulesAsync(string doctorId);
    Task<IEnumerable<DoctorScheduleResponseDto>> GetClinicScheduleAsync(string staffId);
}
