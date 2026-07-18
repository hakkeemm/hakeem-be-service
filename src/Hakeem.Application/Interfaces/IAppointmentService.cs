using Hakeem.Application.DTOs.Appointment;

namespace Hakeem.Application.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentResponseDto> BookAppointmentAsync(string patientId, CreateAppointmentDto dto);
    Task<IEnumerable<AppointmentResponseDto>> GetPatientAppointmentsAsync(string patientId);
    Task<AppointmentResponseDto> GetAppointmentByIdAsync(Guid id, string patientId);
    Task<bool> CancelAppointmentAsync(Guid id, string patientId);
    Task<AppointmentResponseDto> RescheduleAppointmentAsync(Guid id, string patientId, RescheduleAppointmentDto dto);
    Task<AppointmentResponseDto> CheckInPatientAsync(Guid id, string assistantId);
    Task<AppointmentResponseDto> ConfirmAppointmentAsync(Guid id, string staffId);
    Task<AppointmentResponseDto> CompleteAppointmentAsync(Guid id, string staffId);
}
