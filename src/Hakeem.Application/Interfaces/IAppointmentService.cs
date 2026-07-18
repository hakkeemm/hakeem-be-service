using Hakeem.Application.DTOs.Appointment;

namespace Hakeem.Application.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentResponseDto> BookAppointmentAsync(string patientId, CreateAppointmentDto dto);
    Task<IEnumerable<AppointmentResponseDto>> GetPatientAppointmentsAsync(string patientId);
}
