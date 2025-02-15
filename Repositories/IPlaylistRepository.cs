using SoundScape.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoundScape.Repositories
{
    public interface IPlaylistRepository
    {
        Task<List<Playlist>> GetAllPlaylistsAsync();  // Змінили на List<Playlist>
        Task<Playlist> GetPlaylistByIdAsync(int id);
        Task<bool> CreatePlaylistAsync(Playlist playlist); // Додав метод для створення плейлиста
        Task<bool> AddSongToPlaylistAsync(int playlistId, int songId);
        Task<bool> RemoveSongFromPlaylistAsync(int playlistId, int songId);
        Task<bool> DeletePlaylistAsync(int id);
    }
}
