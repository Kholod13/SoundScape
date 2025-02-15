using System.ComponentModel.DataAnnotations.Schema;

namespace SoundScape.Models
{
    public class PlaylistSong
    {
        public int PlaylistId { get; set; }
        public int SongId { get; set; }

        // Навігаційні властивості
        public Playlist Playlist { get; set; }
        public Song Song { get; set; }
    }
}
