using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace NMH.Services
{
    public class FavoritesService
    {
        private readonly IJSRuntime _js;
        private HashSet<int> _favorites = new();

        public event Action? OnChange;

        public FavoritesService(IJSRuntime js)
        {
            _js = js;
        }

        // 🔹 Initialisation depuis le LocalStorage
        public async Task InitializeAsync()
        {
            try
            {
                var json = await _js.InvokeAsync<string>("favorites.get");

                if (!string.IsNullOrEmpty(json))
                {
                    var list = JsonSerializer.Deserialize<List<int>>(json);
                    _favorites = list != null ? new HashSet<int>(list) : new HashSet<int>();
                }
            }
            catch
            {
                _favorites = new HashSet<int>();
            }
        }

        // 🔹 Sauvegarde dans le LocalStorage
        private async Task SaveAsync()
        {
            var list = _favorites.ToList();
            var json = JsonSerializer.Serialize(list);

            await _js.InvokeVoidAsync("favorites.set", list);
        }

        public bool IsFavorite(int id) => _favorites.Contains(id);

        public async Task Toggle(int id)
        {
            if (_favorites.Contains(id))
                _favorites.Remove(id);
            else
                _favorites.Add(id);

            await SaveAsync();
            OnChange?.Invoke();
        }

        public List<int> GetAll() => _favorites.ToList();
    }
}