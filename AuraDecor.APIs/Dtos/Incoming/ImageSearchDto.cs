using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AuraDecor.APIs.Dtos.Incoming
{
    public class ImageSearchDto
    {
        [Required(ErrorMessage = "Image file is required")]
        public IFormFile File { get; set; }

        [Range(1, 50, ErrorMessage = "Limit must be between 1 and 50")]
        public int Limit { get; set; } = 10;

        [StringLength(50, ErrorMessage = "Color filter cannot exceed 50 characters")]
        public string? Color { get; set; }
    }
} 