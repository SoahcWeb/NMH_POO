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

        // 🔎 RECHERCHE MULTI SIMPLE
        public async Task<SearchResponseDto> SearchMultiAsync(string query, int page = 1)
        {
            var response = await _httpClient.GetFromJsonAsync<SearchResponseDto>(
                $"https://api.themoviedb.org/3/search/multi?api_key={_apiKey}&query={Uri.EscapeDataString(query)}&page={page}"
            );

            return response ?? new SearchResponseDto();
        }

        // 🔎 RECHERCHE AVANCÉE POUR BLAZOR
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

        // 🔹 Liste des genres pour dropdown
        public async Task<List<GenreDto>> GetGenresAsync(string mediaType = "movie")
        {
            var response = await _httpClient.GetFromJsonAsync<GenresResponseDto>(
                $"https://api.themoviedb.org/3/genre/{mediaType}/list?api_key={_apiKey}&language=en-US");

            return response?.Genres ?? new List<GenreDto>();
        }

        // 🔹 DTO interne pour TMDB Trending
        private class TmdbTrendingResponse<T>
        {
            public List<T>? Results { get; set; }
        }
    }
}