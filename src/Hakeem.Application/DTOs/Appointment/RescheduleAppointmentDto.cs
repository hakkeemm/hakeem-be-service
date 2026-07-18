namespace Hakeem.Application.DTOs.Appointment;

public class RescheduleAppointmentDto
{
    public DateTime NewAppointmentDate { get; set; }
    public TimeSpan NewStartTime { get; set; }
}
