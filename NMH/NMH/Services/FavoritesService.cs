using System.Net.Http.Json;
using NMH.Shared.DTOs;

namespace NMH.Services
{
    public class FavoritesService
    {
        private readonly HttpClient _http;

        public FavoritesService(HttpClient http)
        {
            _http = http;
        }

        // ⭐ AJOUTER UN FAVORI
        public async Task AddFavorite(int movieId)
        {
            var dto = new FavoriteDto
            {
                MovieId = movieId,
                Comment = ""
            };

            var response = await _http.PostAsJsonAsync("api/favorites", dto);
            response.EnsureSuccessStatusCode();
        }

        // ❌ SUPPRIMER UN FAVORI
        public async Task RemoveFavorite(int movieId)
        {
            var response = await _http.DeleteAsync($"api/favorites/{movieId}");
            response.EnsureSuccessStatusCode();
        }

        // 📥 RÉCUPÉRER LES FAVORIS
        public async Task<List<FavoriteDto>> GetFavorites()
        {
            var result = await _http.GetFromJsonAsync<List<FavoriteDto>>("api/favorites");
            return result ?? new List<FavoriteDto>();
        }

        // ⭐ CHECK SI FAVORI EXISTE
        public async Task<bool> IsFavorite(int movieId)
        {
            var favs = await GetFavorites();
            return favs.Any(f => f.MovieId == movieId);
        }
    }
}