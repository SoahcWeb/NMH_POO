using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NMH.Shared.DTOs
{
    public class SeriesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        [JsonPropertyName("first_air_date")]
        public DateTime? FirstAirDate { get; set; }

        public string OriginalLanguage { get; set; } = string.Empty;
        public double? VoteAverage { get; set; }
        public int? VoteCount { get; set; }
        public List<int> GenreIds { get; set; } = new();

        public string PosterFullPath =>
            string.IsNullOrEmpty(PosterPath) ? "" : $"https://image.tmdb.org/t/p/w500{PosterPath}";
    }
}