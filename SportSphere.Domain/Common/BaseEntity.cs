public abstract class BaseEntity : IBaseEntity
{

    public long Id { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted => DeletedAt.HasValue;
}