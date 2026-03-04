using System;

namespace NMH.Shared.DTOs
{
    /// <summary>
    /// Represents a TV series from TMDB API.
    /// </summary>
    public class SeriesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public string PosterPath { get; set; } = string.Empty;
        public DateTime? FirstAirDate { get; set; }
        public string OriginalLanguage { get; set; } = string.Empty;
        public double? VoteAverage { get; set; }
        public int? VoteCount { get; set; }
        public string? BackdropPath { get; set; }

        /// <summary>
        /// Full URL to the series poster.
        /// </summary>
        public string PosterFullPath => 
            string.IsNullOrEmpty(PosterPath) ? "" : $"https://image.tmdb.org/t/p/w500{PosterPath}";
    }
}