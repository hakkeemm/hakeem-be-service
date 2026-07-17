using Hakeem.Domain.Common;

namespace Hakeem.Domain.Entities;

public class OnlineMeeting : BaseAuditableEntity
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;
    
    public string? MeetingProvider { get; set; } // e.g. Zoom, GoogleMeet
    public string? MeetingUrl { get; set; }
    public string? MeetingId { get; set; }
    public string? MeetingPassword { get; set; }
    
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
}
