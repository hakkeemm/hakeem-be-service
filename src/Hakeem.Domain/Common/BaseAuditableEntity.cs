namespace Hakeem.Domain.Common;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    string? CreatedById { get; set; }
    DateTime? UpdatedAt { get; set; }
    string? UpdatedById { get; set; }
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    string? DeletedById { get; set; }
}

public abstract class BaseAuditableEntity : IAuditableEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedById { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedById { get; set; }
    
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }
}
