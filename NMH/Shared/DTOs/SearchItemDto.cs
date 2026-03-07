using System;
using System.Text.Json.Serialization;

namespace NMH.Shared.DTOs
{
    public class SearchItemDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("media_type")]
        public string? MediaType { get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }  // Film

        [JsonPropertyName("first_air_date")]
        public string? FirstAirDate { get; set; } // Série
    }
}