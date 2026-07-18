using Hakeem.Domain.Common;

namespace Hakeem.Domain.Entities;

public class DoctorSchedule : BaseAuditableEntity
{
    public Guid Id { get; set; }
    public string DoctorId { get; set; } = string.Empty;
    public ApplicationUser Doctor { get; set; } = null!;
    
    public System.DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int SlotDurationMinutes { get; set; }
}
