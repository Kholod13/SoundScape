using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundScape.Data;
using SoundScape.Models;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class SearchController : Controller
{
    private readonly ApplicationDbContext _context;

    public SearchController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { message = "Search query is required." });
        }

        var tracks = await _context.MusicTracks
            .Where(t => t.Title.Contains(q) || t.Artist.Contains(q))
            .Select(t => new
            {
                id = t.Id,
                type = "Track",
                name = t.Title,
                artist = t.Artist,
                year = t.UploadDate.Year,
                songsCount = 1,
                image = t.ImageUrl,
                audio = t.FilePath
            })
            .ToListAsync();

        if (!tracks.Any())
        {
            return NotFound(new { message = "No results found." });
        }

        return Ok(new { results = tracks });
    }
}
