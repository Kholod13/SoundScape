namespace SoundScape.Models
{
    public class Playlist
    {
        public int Id { get; set; }

        // Ініціалізуємо властивість Name за замовчуванням
        public string Name { get; set; } = string.Empty;

        // Ініціалізуємо властивість Songs за замовчуванням
        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
