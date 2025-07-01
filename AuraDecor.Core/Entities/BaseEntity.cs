namespace AuraDecor.Core.Entities;

public class BaseEntity
{
    public Guid Id { get; set; } = new Guid();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}