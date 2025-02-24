using SoundScape.Models;

public class Song
{
    public int Id { get; set; }
    public string Title { get; set; }
    public TimeSpan Duration { get; set; }
    public string FilePath { get; set; }
    public int AlbumId { get; set; }
    public Album Album { get; set; }

    public List<SongGenre> SongGenres { get; set; } = new List<SongGenre>();
    public ICollection<PlaylistSong> PlaylistSongs { get; set; }

    public ICollection<SongArtist> SongArtists { get; set; } // тут встановлюється зв'язок з артистами
}
