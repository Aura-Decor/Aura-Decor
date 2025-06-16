using System.ComponentModel.DataAnnotations;

namespace AuraDecor.APIs.Dtos.Incoming;

public class CreateStyleTypeDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; }
}
