namespace SoundScape.DTOs
{
    public class SongDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Genre { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int DurationInSeconds { get; set; }
    }
}
