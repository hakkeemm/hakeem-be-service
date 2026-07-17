namespace Hakeem.Domain.Entities;

public class Review
{
    public Guid Id { get; set; }
    
    public string PatientId { get; set; } = string.Empty;
    public ApplicationUser Patient { get; set; } = null!;
    
    public string DoctorId { get; set; } = string.Empty;
    public ApplicationUser Doctor { get; set; } = null!;
    
    public Guid AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;
    
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
