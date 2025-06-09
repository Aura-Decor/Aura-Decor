using System;

namespace AuraDecor.APIs.Dtos.Outgoing
{
    public class RatingDto
    {
        public Guid Id { get; set; }
        public int Stars { get; set; }
        public string Review { get; set; }
        public string UserDisplayName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 