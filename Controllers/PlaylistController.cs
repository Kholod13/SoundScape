using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundScape.Models;
using SoundScape.Data;  // Додайте цей using
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class PlaylistController : ControllerBase
{
    private readonly SoundScapeDbContext _context;

    public PlaylistController(SoundScapeDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlaylist([FromBody] Playlist playlist)
    {
        _context.Playlists.Add(playlist);
        await _context.SaveChangesAsync();
        return Ok(playlist);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlaylist(int id)
    {
        var playlist = await _context.Playlists.Include(p => p.Songs).FirstOrDefaultAsync(p => p.Id == id);
        if (playlist == null)
        {
            return NotFound();
        }
        return Ok(playlist);
    }
}
