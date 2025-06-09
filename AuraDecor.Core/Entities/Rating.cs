using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuraDecor.Core.Entities;

public class Rating : BaseEntity
{
    public int Stars { get; set; }
    
    public string Review { get; set; }
    
    public string UserId { get; set; }
    
    public User User { get; set; }
    
    public Guid ProductId { get; set; }
    
    public Furniture Product { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 