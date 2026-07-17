namespace Hakeem.Domain.Entities;

public class DoctorScheduleException
{
    public Guid Id { get; set; }
    public string DoctorId { get; set; } = string.Empty;
    public ApplicationUser Doctor { get; set; } = null!;
    
    public DateTime ExceptionDate { get; set; }
    public bool IsAvailable { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
}
