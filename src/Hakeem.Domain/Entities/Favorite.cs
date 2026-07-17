using Hakeem.Domain.Common;

namespace Hakeem.Domain.Entities;

public class Favorite : BaseAuditableEntity
{
    public Guid Id { get; set; }
    
    public string PatientId { get; set; } = string.Empty;
    public ApplicationUser Patient { get; set; } = null!;
    
    public string DoctorId { get; set; } = string.Empty;
    public ApplicationUser Doctor { get; set; } = null!;
}
