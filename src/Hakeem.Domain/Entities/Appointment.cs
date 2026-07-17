using Hakeem.Domain.Enums;
using Hakeem.Domain.Common;

namespace Hakeem.Domain.Entities;

public class Appointment : BaseAuditableEntity
{
    public Guid Id { get; set; }
    public string PatientId { get; set; } = string.Empty;
    public ApplicationUser Patient { get; set; } = null!;
    
    public string DoctorId { get; set; } = string.Empty;
    public ApplicationUser Doctor { get; set; } = null!;
    public DateTime AppointmentDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    
    public string VisitType { get; set; } = string.Empty; // e.g. "InPerson", "Online"
    public string VisitPurpose { get; set; } = string.Empty; // e.g. "NewVisit", "FollowUp"
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Booked;

    public decimal Fee { get; set; }
    public string? Notes { get; set; }
    public OnlineMeeting? OnlineMeeting { get; set; }
    public Review? Review { get; set; }

}
