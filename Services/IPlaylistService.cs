using SoundScape.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoundScape.Services
{
    public interface IPlaylistService
    {
        Task<IEnumerable<Playlist>> GetAllPlaylistsAsync();
        Task<Playlist> GetPlaylistByIdAsync(int id);
        Task CreatePlaylistAsync(Playlist playlist);
        Task DeletePlaylistAsync(int id);
        Task AddSongToPlaylistAsync(int playlistId, int songId);
        Task RemoveSongFromPlaylistAsync(int playlistId, int songId);
    }
}
