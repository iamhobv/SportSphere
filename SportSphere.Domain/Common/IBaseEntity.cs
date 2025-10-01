namespace MagiXSquad.Domain.Common;
public interface IBaseEntity
{
    [Key]
    public long Id { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted => DeletedAt.HasValue;
}
