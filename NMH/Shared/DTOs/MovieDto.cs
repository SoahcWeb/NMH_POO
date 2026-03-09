using System;
using System.Text.Json.Serialization;

namespace NMH.Shared.DTOs
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; } = string.Empty;

        [JsonPropertyName("release_date")]
        public DateTime? ReleaseDate { get; set; }

        public string OriginalLanguage { get; set; } = string.Empty;
        public double? VoteAverage { get; set; }
        public int? VoteCount { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        public string PosterFullPath =>
            string.IsNullOrEmpty(PosterPath) ? "" : $"https://image.tmdb.org/t/p/w500{PosterPath}";
    }
}