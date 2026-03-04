using System.Net.Http.Json;
using System.Text.Json.Serialization;
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
            _apiKey = _configuration["TMDB:ApiKey"];
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

        // 🔹 Wrapper générique pour la réponse TMDB
        private class TmdbTrendingResponse<T>
        {
            [JsonPropertyName("results")]
            public List<T>? Results { get; set; }
        }
    }
}