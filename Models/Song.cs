namespace SoundScape.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public decimal Duration { get; set; }
        public string FilePath { get; set; }

        // Зворотній зв'язок для пісень
        public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
    }
}
