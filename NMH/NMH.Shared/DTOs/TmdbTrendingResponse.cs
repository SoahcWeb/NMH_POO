using System.Collections.Generic;

namespace NMH.Shared.DTOs
{
    public class TmdbTrendingResponse<T>
    {
        public int Page { get; set; }
        public List<T> Results { get; set; } = new List<T>();
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
    }
}