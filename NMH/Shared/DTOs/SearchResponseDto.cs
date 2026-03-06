using System.Text.Json.Serialization;

namespace NMH.Shared.DTOs
{
    public class SearchResponseDto
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("results")]
        public List<SearchItemDto> Results { get; set; } = new();
    }
}