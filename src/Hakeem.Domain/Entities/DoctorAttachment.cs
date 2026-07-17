using Hakeem.Domain.Common;

namespace Hakeem.Domain.Entities;

public class DoctorAttachment : BaseAuditableEntity
{
    public Guid Id { get; set; }
    
    public string DoctorId { get; set; } = string.Empty;
    public ApplicationUser Doctor { get; set; } = null!;
    
    public string Title { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? FileType { get; set; }
}
