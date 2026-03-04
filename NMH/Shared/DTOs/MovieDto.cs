using System;

namespace NMH.Shared.DTOs
{
    /// <summary>
    /// Represents a movie from TMDB API.
    /// </summary>
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public string PosterPath { get; set; } = string.Empty;
        public DateTime? ReleaseDate { get; set; }
        public string OriginalLanguage { get; set; } = string.Empty;
        public double? VoteAverage { get; set; }
        public int? VoteCount { get; set; }
        public string? BackdropPath { get; set; }

        /// <summary>
        /// Full URL to the movie poster.
        /// </summary>
        public string PosterFullPath => 
            string.IsNullOrEmpty(PosterPath) ? "" : $"https://image.tmdb.org/t/p/w500{PosterPath}";
    }
}