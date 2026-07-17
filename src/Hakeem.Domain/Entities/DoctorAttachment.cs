namespace Hakeem.Domain.Entities;

public class DoctorAttachment
{
    public Guid Id { get; set; }
    
    public string DoctorId { get; set; } = string.Empty;
    public ApplicationUser Doctor { get; set; } = null!;
    
    public string Title { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? FileType { get; set; }
}
