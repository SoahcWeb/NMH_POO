using System;

namespace NMH.Shared.DTOs
{
    public class SeriesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public string? PosterPath { get; set; }
        public string? BackdropPath { get; set; }
        public DateTime? FirstAirDate { get; set; }
        public string OriginalLanguage { get; set; } = string.Empty;
        public double? VoteAverage { get; set; }
        public int? VoteCount { get; set; }
        public List<int> GenreIds { get; set; } = new();

        public string PosterFullPath =>
            string.IsNullOrEmpty(PosterPath) ? "" : $"https://image.tmdb.org/t/p/w500{PosterPath}";
    }
}