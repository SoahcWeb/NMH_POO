using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using NMH.Shared.Converters;

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
        [JsonConverter(typeof(JsonDateConverter))]
        public DateTime? ReleaseDate { get; set; }

        public string OriginalLanguage { get; set; } = string.Empty;

        public double? VoteAverage { get; set; }
        public int? VoteCount { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        public string? Tagline { get; set; }

        public int Runtime { get; set; }

        public List<GenreDto>? Genres { get; set; }
        public string GenresText { get; set; } = string.Empty;

        public List<ProductionCompanyDto>? ProductionCompanies { get; set; }
        public string ProductionCompaniesText { get; set; } = string.Empty;

        public string? TrailerUrl { get; set; }

        public List<CastDto> Cast { get; set; } = new();

        public List<MovieDto> SimilarMovies { get; set; } = new();

        public string PosterFullPath =>
            string.IsNullOrEmpty(PosterPath)
                ? ""
                : $"https://image.tmdb.org/t/p/w500{PosterPath}";

        // ✅ AJOUT
        public string BackdropFullPath =>
            string.IsNullOrEmpty(BackdropPath)
                ? ""
                : $"https://image.tmdb.org/t/p/original{BackdropPath}";
    }
}