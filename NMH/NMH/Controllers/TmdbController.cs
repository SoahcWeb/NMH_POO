using Microsoft.AspNetCore.Mvc;
using NMH.Services;
using NMH.Shared.DTOs;

namespace NMH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TmdbController : ControllerBase
    {
        private readonly TmdbService _tmdbService;

        public TmdbController(TmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        // 🔹 Obtenir les films trending
        [HttpGet("movies")]
        public async Task<IActionResult> GetTrendingMovies()
        {
            var movies = await _tmdbService.GetTrendingMoviesAsync();
            return Ok(movies);
        }

        // 🔹 Obtenir les séries trending
        [HttpGet("series")]
        public async Task<IActionResult> GetTrendingSeries()
        {
            var series = await _tmdbService.GetTrendingSeriesAsync();
            return Ok(series);
        }
    }
}