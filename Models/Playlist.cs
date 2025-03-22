using SoundScape.Models;

public class Playlist
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public ICollection<PlaylistSong> PlaylistSongs { get; set; } // Це з'єднує Playlist і Song
}
