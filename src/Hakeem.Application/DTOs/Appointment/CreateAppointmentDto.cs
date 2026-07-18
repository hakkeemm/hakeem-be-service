using Hakeem.Domain.Enums;

namespace Hakeem.Application.DTOs.Appointment;

public class CreateAppointmentDto
{
    public string DoctorId { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public VisitType VisitType { get; set; }
    public VisitPurpose VisitPurpose { get; set; }
    public string? Notes { get; set; }
}
