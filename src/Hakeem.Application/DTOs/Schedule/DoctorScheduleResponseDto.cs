namespace Hakeem.Application.DTOs.Schedule;

public class DoctorScheduleResponseDto
{
    public Guid Id { get; set; }
    public string DoctorId { get; set; } = string.Empty;
    public System.DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int SlotDurationMinutes { get; set; }
}
