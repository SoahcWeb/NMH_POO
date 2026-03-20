using System.Net.Http.Json;
using NMH.Shared.DTOs;

namespace NMH.Services
{
    public class TmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;

        public TmdbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiKey = _configuration["TMDB:ApiKey"] ?? throw new InvalidOperationException("TMDB:ApiKey missing in configuration");
        }

        public async Task<List<MovieDto>> GetTrendingMoviesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<TmdbTrendingResponse<MovieDto>>(
                $"https://api.themoviedb.org/3/trending/movie/week?api_key={_apiKey}"
            );

            return response?.Results ?? new List<MovieDto>();
        }

        public async Task<List<SeriesDto>> GetTrendingSeriesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<TmdbTrendingResponse<SeriesDto>>(
                $"https://api.themoviedb.org/3/trending/tv/week?api_key={_apiKey}"
            );

            return response?.Results ?? new List<SeriesDto>();
        }

        public async Task<List<MovieDto>> GetPopularMoviesAsync(int count = 10)
        {
            var response = await _httpClient.GetFromJsonAsync<TmdbTrendingResponse<MovieDto>>(
                $"https://api.themoviedb.org/3/movie/popular?api_key={_apiKey}&language=fr-FR&page=1"
            );
            return response?.Results?.Take(count).ToList() ?? new List<MovieDto>();
        }

        public async Task<List<SeriesDto>> GetPopularSeriesAsync(int count = 10)
        {
            var response = await _httpClient.GetFromJsonAsync<TmdbTrendingResponse<SeriesDto>>(
                $"https://api.themoviedb.org/3/tv/popular?api_key={_apiKey}&language=fr-FR&page=1"
            );
            return response?.Results?.Take(count).ToList() ?? new List<SeriesDto>();
        }

        public async Task<SearchResponseDto> SearchMultiAsync(string query, int page = 1)
        {
            var response = await _httpClient.GetFromJsonAsync<SearchResponseDto>(
                $"https://api.themoviedb.org/3/search/multi?api_key={_apiKey}&query={Uri.EscapeDataString(query)}&page={page}"
            );

            return response ?? new SearchResponseDto();
        }

        public async Task<SearchResponseDto> SearchAsync(
            string query,
            string mediaType = "movie",
            int? year = null,
            string? actor = null,
            int? genreId = null,
            int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query) && !year.HasValue && string.IsNullOrWhiteSpace(actor) && !genreId.HasValue)
                return new SearchResponseDto();

            var url = $"https://api.themoviedb.org/3/search/{mediaType}?api_key={_apiKey}&page={page}";

            if (!string.IsNullOrWhiteSpace(query))
                url += $"&query={Uri.EscapeDataString(query)}";

            if (year.HasValue)
                url += $"&year={year.Value}";

            if (genreId.HasValue)
                url += $"&with_genres={genreId.Value}";

            if (!string.IsNullOrWhiteSpace(actor))
            {
                var actorResponse = await _httpClient.GetFromJsonAsync<SearchResponseDto>(
                    $"https://api.themoviedb.org/3/search/person?api_key={_apiKey}&query={Uri.EscapeDataString(actor)}");

                var actorId = actorResponse?.Results?.FirstOrDefault()?.Id;
                if (actorId.HasValue)
                    url += $"&with_cast={actorId.Value}";
            }

            var response = await _httpClient.GetFromJsonAsync<SearchResponseDto>(url);
            return response ?? new SearchResponseDto();
        }

        public async Task<List<GenreDto>> GetGenresAsync(string mediaType = "movie")
        {
            var response = await _httpClient.GetFromJsonAsync<GenresResponseDto>(
                $"https://api.themoviedb.org/3/genre/{mediaType}/list?api_key={_apiKey}&language=en-US");

            return response?.Genres ?? new List<GenreDto>();
        }

        // =====================================================
        // ✅ GET MOVIE BY ID (ENRICHI)
        // =====================================================
        public async Task<MovieDto?> GetMovieByIdAsync(int id)
        {
            var movie = await _httpClient.GetFromJsonAsync<MovieDto>(
                $"https://api.themoviedb.org/3/movie/{id}?api_key={_apiKey}&language=fr-FR"
            );

            if (movie == null)
                return null;

            // Credits
            var credits = await _httpClient.GetFromJsonAsync<CreditsResponse>(
                $"https://api.themoviedb.org/3/movie/{id}/credits?api_key={_apiKey}&language=fr-FR"
            );

            if (credits?.Cast != null)
            {
                movie.Cast = credits.Cast.Select(c => new CastDto
                {
                    Name = c.Name,
                    ProfilePath = c.ProfilePath
                }).ToList();
            }

            // Similar movies
            var similar = await _httpClient.GetFromJsonAsync<TmdbTrendingResponse<MovieDto>>(
                $"https://api.themoviedb.org/3/movie/{id}/similar?api_key={_apiKey}&language=fr-FR&page=1"
            );

            movie.SimilarMovies = similar?.Results ?? new List<MovieDto>();

            // Videos (Trailer)
            var videos = await _httpClient.GetFromJsonAsync<VideosResponse>(
                $"https://api.themoviedb.org/3/movie/{id}/videos?api_key={_apiKey}&language=fr-FR"
            );

            var trailer = videos?.Results?
                .FirstOrDefault(v => v.Type == "Trailer" && v.Site == "YouTube");

            if (trailer != null)
            {
                movie.TrailerUrl = $"https://www.youtube.com/watch?v={trailer.Key}";
            }

            // Genres text
            if (movie.Genres != null)
            {
                movie.GenresText = string.Join(", ", movie.Genres.Select(g => g.Name));
            }

            // Production companies text
            if (movie.ProductionCompanies != null)
            {
                movie.ProductionCompaniesText = string.Join(", ", movie.ProductionCompanies.Select(p => p.Name));
            }

            return movie;
        }

        public async Task<SeriesDto?> GetSeriesByIdAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<SeriesDto>(
                $"https://api.themoviedb.org/3/tv/{id}?api_key={_apiKey}&language=fr-FR"
            );

            return response;
        }

        // =====================================================
        // INTERNAL DTOs
        // =====================================================

        private class TmdbTrendingResponse<T>
        {
            public List<T>? Results { get; set; }
        }

        private class CreditsResponse
        {
            public List<CastItem>? Cast { get; set; }
        }

        private class CastItem
        {
            public string Name { get; set; } = string.Empty;
            public string? ProfilePath { get; set; }
        }

        private class VideosResponse
        {
            public List<VideoItem>? Results { get; set; }
        }

        private class VideoItem
        {
            public string Key { get; set; } = string.Empty;
            public string Site { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
        }
    }
}