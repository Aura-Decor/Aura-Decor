using System.ComponentModel.DataAnnotations;

namespace AuraDecor.APIs.Dtos.Incoming
{
    public class AddRatingDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Stars { get; set; }

        public string Review { get; set; }
    }
} 