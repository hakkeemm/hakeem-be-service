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
    
    public VisitType VisitType { get; set; } = VisitType.InPerson;
    public VisitPurpose VisitPurpose { get; set; } = VisitPurpose.NewVisit;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Booked;

    public decimal Fee { get; set; }
    public string? Notes { get; set; }
    public OnlineMeeting? OnlineMeeting { get; set; }
    public Review? Review { get; set; }

}
