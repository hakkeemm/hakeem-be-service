using Hakeem.Domain.Common;

namespace Hakeem.Domain.Entities;

public class Category : BaseAuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    
    public ICollection<ApplicationUser> Doctors { get; set; } = new List<ApplicationUser>();
}
