using Hakeem.Application.DTOs.Appointment;

namespace Hakeem.Application.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentResponseDto> BookAppointmentAsync(string patientId, CreateAppointmentDto dto);
    Task<IEnumerable<AppointmentResponseDto>> GetPatientAppointmentsAsync(string patientId);
    Task<Hakeem.Application.DTOs.Common.PaginatedList<AppointmentResponseDto>> GetClinicAppointmentsAsync(string staffId, Hakeem.Application.DTOs.Appointment.AppointmentFilterDto filter);
    Task<AppointmentResponseDto> GetAppointmentByIdAsync(Guid id, string patientId);
    Task<bool> CancelAppointmentAsync(Guid id, string patientId);
    Task<AppointmentResponseDto> RescheduleAppointmentAsync(Guid id, string patientId, RescheduleAppointmentDto dto);
    Task<AppointmentResponseDto> UpdateAppointmentStatusAsync(Guid id, string staffId, Hakeem.Domain.Enums.AppointmentStatus newStatus);
}
