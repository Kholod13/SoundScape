using System.Text.Json.Serialization;

namespace SoundScape.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Колекція для зв'язку з піснями через таблицю PlaylistSong
        [JsonIgnore] // Запобігає циклічній серіалізації
        public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
    }
}
