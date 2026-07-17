using Hakeem.Domain.Common;

namespace Hakeem.Domain.Entities;

public class DoctorSchedule : BaseAuditableEntity
{
    public Guid Id { get; set; }
    public string DoctorId { get; set; } = string.Empty;
    public ApplicationUser Doctor { get; set; } = null!;
    
    public int DayOfWeek { get; set; } // 0 = Sunday, 1 = Monday, etc.
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int SlotDurationMinutes { get; set; }
}
