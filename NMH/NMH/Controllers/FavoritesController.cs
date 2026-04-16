using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMH.Data;

namespace NMH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🟢 GET (TEST SANS AUTH)
        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = "test-user";

            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId)
                .ToListAsync();

            return Ok(favorites);
        }

        // 🟢 POST (CORRIGÉ SAFE MODEL BINDING)
        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] Favorite fav)
        {
            if (fav == null)
                return BadRequest();

            // 🔥 DEBUG AJOUTÉ
            Console.WriteLine("🔥 ADD FAVORITE CALLED");
            Console.WriteLine($"MovieId = {fav.MovieId}");
            Console.WriteLine($"Comment = {fav.Comment}");

            var userId = "test-user";

            var newFav = new Favorite
            {
                MovieId = fav.MovieId,
                Comment = fav.Comment,
                UserId = userId
            };

            _context.Favorites.Add(newFav);
            await _context.SaveChangesAsync();

            return Ok(newFav);
        }

        // 🟡 PUT (TEST SANS AUTH)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] string comment)
        {
            var userId = "test-user";

            var fav = await _context.Favorites
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            if (fav == null)
                return NotFound();

            fav.Comment = comment;

            await _context.SaveChangesAsync();

            return Ok(fav);
        }

        // 🔴 DELETE (TEST SANS AUTH)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            var userId = "test-user";

            var fav = await _context.Favorites
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            if (fav == null)
                return NotFound();

            _context.Favorites.Remove(fav);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}