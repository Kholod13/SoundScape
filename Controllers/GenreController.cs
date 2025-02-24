using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundScape.Data;
using SoundScape.Models;
using SoundScape.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace SoundScape.Controllers
{
    [ApiController]
    [Route("api/genres")]
    public class GenreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenreController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Отримати всі жанри
        [HttpGet]
        public async Task<IActionResult> GetAllGenres()
        {
            var genres = await _context.Genres
                .Select(g => new GenreDTO
                {
                    Id = g.Id,
                    Name = g.Name
                })
                .ToListAsync();

            return Ok(genres);
        }

        // Отримати жанр за ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGenreById(int id)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);

            if (genre == null)
                return NotFound();

            var result = new GenreDTO
            {
                Id = genre.Id,
                Name = genre.Name
            };

            return Ok(result);
        }

        // Створити новий жанр
        [HttpPost]
        public async Task<IActionResult> CreateGenre([FromBody] GenreDTO genreDTO)
        {
            // Перевірка на null або порожнє ім'я
            if (genreDTO == null || string.IsNullOrWhiteSpace(genreDTO.Name))
            {
                return BadRequest("Genre name is required.");
            }

            // Створення нового жанру
            var genre = new Genre
            {
                Name = genreDTO.Name
            };

            // Додавання жанру до бази даних
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            // Повертаємо відповідь про успішне створення жанру
            return CreatedAtAction(nameof(GetGenreById), new { id = genre.Id }, genre);
        }

        // Оновити жанр
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(int id, [FromBody] GenreDTO genreDTO)
        {
            // Перевірка на null або невідповідність Id
            if (genreDTO == null || genreDTO.Id != id)
            {
                return BadRequest("Genre ID mismatch.");
            }

            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);
            if (genre == null)
            {
                return NotFound();
            }

            genre.Name = genreDTO.Name;
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Видалити жанр
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);
            if (genre == null)
            {
                return NotFound();
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
