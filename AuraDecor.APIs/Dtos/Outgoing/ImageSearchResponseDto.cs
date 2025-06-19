namespace AuraDecor.APIs.Dtos.Outgoing
{
    public class ImageSearchResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<ImageSearchResultDto> Results { get; set; } = new List<ImageSearchResultDto>();
    }
} 